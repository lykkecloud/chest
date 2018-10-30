"""Reading data from Postgres DB into CSV file and loading it into MsSql DB"""

import pandas
import sqlalchemy


def getSqlEngine(connType, server, port, database, username, pwd, driver=''):
    try:
        connString = "{}://{}:{}@{}:{}/{}".format(connType, username, pwd, server, port, database)
        if driver: connString = "{}?driver={}".format(connString, driver)
        return sqlalchemy.create_engine(connString)

    except Exception as exception:
        print ("Error creating Sql Engine: {0}".format(exception))    


def exportToCsv(engine, queryToGetData, indexColumn, fileName):
    try:
        df = pandas.read_sql(queryToGetData, engine, indexColumn)
        df.to_csv(fileName)

    except Exception as exception:
        print ("Error exporting data from to CSV: {0}".format(exception))


def importFromCsv(engine, schema, tableName, fileName):
    try:
        df = pandas.read_csv(fileName)
        df.to_sql(tableName, engine, schema, 'append', False)

    except Exception as exception:
        print ("Error loading data from CSV: {0}".format(exception))


def migrateFromPostgresToMsSql():
    try:
        queryToGetData = """
            SELECT
                key as Key,
                display_key as DisplayKey,
                meta_data as Metadata,
                category as Category,
                collection as Collection,
                display_category as DisplayCategory,
                display_collection as DisplayCollection,
                keywords as Keywords
            FROM key_value_data
        """
        
        csvFileName = 'ChestDataFromPostgres.csv'
        
        postgresEngine = getSqlEngine('postgresql+psycopg2', 'postgres.server.url', '5432', 'dbName', 'username', 'password')
        msSqlEngine = getSqlEngine('mssql+pyodbc', 'mssql.server.url', '1433', 'dbName', 'username', 'password', 'SQL+Server')
        
        exportToCsv(postgresEngine, queryToGetData, 'key', csvFileName)
        importFromCsv(msSqlEngine, 'chest', 'tb_keyValueData', csvFileName)

    except Exception as exception:
        print ("Error migrating data from Postgres to MsSql: {0}".format(exception))


if __name__ == "__main__":
    migrateFromPostgresToMsSql()
