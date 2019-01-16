import os
import pyodbc
import json
import itertools
import re

import click

# Export tool for StaticSQL - extracts live SQL Server tables into metadata files.

extract_query = """
SELECT
    s.name AS schema_name
  , t.name AS table_name
  , c.name AS column_name
  , TYPE_NAME(system_type_id) AS type_name
  , CASE
        WHEN max_length = -1 THEN
            'MAX' 
        WHEN TYPE_NAME(system_type_id) IN ('NVARCHAR', 'NCHAR') THEN
            CAST(max_length/2 AS NVARCHAR(100))
        ELSE
            CAST(max_length AS NVARCHAR(100))
    END AS max_length
  , precision
  , c.is_nullable
FROM sys.columns AS c
INNER JOIN sys.tables AS t
  ON t.object_id = c.object_id
INNER JOIN sys.schemas AS s
  ON s.schema_id = t.schema_id
ORDER BY 
    s.name
  , t.name
  , c.column_id
"""

type_formats = {
    "bit": "BIT",
    "int": "INT",
    "bigint": "BIGINT",
    "smallint": "SMALLINT",
    "tinyint": "TINYINT",
    "float": "FLOAT",
    "decimal": "DECIMAL({precision},{max_length})",
    "date": "DATE",
    "datetime": "DATETIME",
    "nvarchar": "NVARCHAR({max_length})",
    "varchar": "VARCHAR({max_length})",
    "nchar": "NCHAR({max_length})",
    "char": "CHAR({max_length})",
    "uniqueidentifier": "UNIQUEIDENTIFIER",
}

@click.command()
@click.option("-S", "--server", default=lambda: os.environ.get("COMPUTERNAME", "."), help="Name of the source server")
@click.option("-d", "--database", default="master", help="Name of the source database")
@click.option("-s", "--schemas", default=".*", help="Regex to select schemas to extract from (defaults to .*)")
@click.option("-t", "--tables", default=".*", help="Regex to select tables to extract (defaults to .*)")
@click.option("-c", "--columns", default=".*", help='Regex to select columns, e.g. "^(?!ABC)" to ignore columns starting with "ABC"')
@click.option("-ts", "--target-schema", default=None, help="Value to use for the schema attribute instead of source schema")
@click.option("-e", "--extension", default=".json", help="Extension of the output file (defaults to .json)")
@click.option("-i", "--indent", default=2, help="Number of spaces to indent each level of the json output")
@click.option("-v", "--verbose", is_flag=True, help="More verbose output")
def staticsql(server, database, schemas, target_schema, tables, columns, extension, indent, verbose):
    connection_info = dict()
    connection_info["Driver"] = "{SQL Server}"
    connection_info["Server"] = server
    connection_info["Database"] = database
    connection_info["Trusted_Connection"] = "True"
    connection_string = ";".join("=".join(item) for item in connection_info.items())
    if verbose:
        click.echo("Connection string: {0}".format(connection_string))
    click.echo("Connecting to server [{0}], database [{1}]...".format(server, database))
    conn = pyodbc.connect(connection_string, autocommit=True)
    if verbose:
        click.echo("Connected")
    cursor = conn.cursor()
    cursor.execute("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED")
    cursor.execute(extract_query)

    for (schema_name,), schema_columns in itertools.groupby(cursor.fetchall(), lambda row: row[0:1]):
        if not re.match(schemas, schema_name):
            click.echo("Skipping schema [%s]" % schema_name)
            continue
        if verbose:
            click.echo("Processing schema [%s]" % schema_name)

        for (table_name,), table_columns in itertools.groupby(schema_columns, lambda row: row[1:2]):
            if not re.match(tables, table_name):
                click.echo("Skipping table [%s]" % table_name)
                continue
            if verbose:
                click.echo("Processing table [%s]" % table_name)

            table_dict = {"schema": target_schema or schema_name, "name": table_name, "attributes": []}
            skipped = list()
            for _, _, column_name, type_name, max_length, precision, is_nullable in table_columns:
                if not re.match(columns, column_name):
                    skipped.append(column_name)
                    continue
                column_dict = dict()
                table_dict["attributes"].append(column_dict)
                column_dict["name"] = column_name
                column_dict["data_type"] = type_formats[type_name].format(max_length=max_length, precision=precision)
                column_dict["is_nullable"] = bool(is_nullable)
            if skipped:
                click.echo("Skipped columns %s" % ", ".join("[%s]" % col for col in skipped))
            file_name = "{0}.{1}{2}".format(schema_name, table_name, extension)
            with open(file_name, 'w') as f:
                json.dump(table_dict, f, indent=indent)
    conn.close()


if __name__=="__main__":
    staticsql()
