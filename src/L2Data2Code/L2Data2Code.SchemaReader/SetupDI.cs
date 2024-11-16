using L2Data2Code.SchemaReader.Configuration;
using L2Data2Code.SchemaReader.Fake;
using L2Data2Code.SchemaReader.Interface;
using L2Data2Code.SchemaReader.Json;
using L2Data2Code.SchemaReader.MySql;
using L2Data2Code.SchemaReader.NpgSql;
using L2Data2Code.SchemaReader.Object;
using L2Data2Code.SchemaReader.Schema;
using L2Data2Code.SchemaReader.SqlServer;
using L2Data2Code.SharedLib.Configuration;
using Npgsql;
using Topelab.Core.Resolver.Entities;

namespace L2Data2Code.SchemaReader
{
    public class SetupDI
    {
        private static bool IsLoaded;

        public static ResolveInfoCollection Register()
        {
            if (IsLoaded)
            {
                return [];
            }

            IsLoaded = true;

            return new ResolveInfoCollection()
                .AddSingleton<IBasicConfiguration<SchemaConfiguration>, SchemasConfiguration>()
                .AddSingleton<ISchemaOptionsFactory, SchemaOptionsFactory>()
                .AddSingleton<ISchemaService, SchemaService>()
                .AddSingleton<INameResolver, NameResolver>()
                .AddSingleton<ISchemaFactory, SchemaFactory>()
                .AddTransient<ISchemaReader, SqlServerSchemaReader>(nameof(SqlServerSchemaReader), typeof(INameResolver), typeof(ISchemaOptions))
                .AddTransient<ISchemaReader, MySqlSchemaReader>(nameof(MySqlSchemaReader), typeof(INameResolver), typeof(ISchemaOptions))
                .AddTransient<ISchemaReader, FakeSchemaReader>(nameof(FakeSchemaReader), typeof(INameResolver), typeof(ISchemaOptions))
                .AddTransient<ISchemaReader, JsonSchemaReader>(nameof(JsonSchemaReader), typeof(INameResolver), typeof(ISchemaOptions))
                .AddTransient<ISchemaReader, ObjectSchemaReader>(nameof(ObjectSchemaReader), typeof(INameResolver), typeof(ISchemaOptions))
                .AddTransient<ISchemaReader, SQLiteSchemaReader>(nameof(SQLiteSchemaReader), typeof(INameResolver), typeof(ISchemaOptions))
                .AddTransient<ISchemaReader, NpgSchemaReader>(nameof(NpgSchemaReader), typeof(INameResolver), typeof(ISchemaOptions))

                .AddTransient<IForeignKeysGetter<NpgsqlConnection>, NpgForeignKeysGetter>()
                .AddTransient<IColumnsGetter<NpgsqlConnection>, NpgColumnsGetter>()
                .AddTransient<IColumnDescriptionsGetter<NpgsqlConnection>, NpgColumnDescriptionsGetter>()
                .AddTransient<IColumnIdentitiesGetter<NpgsqlConnection>, NpgColumnIdentitiesGetter>()
                .AddTransient<IIndexesGetter<NpgsqlConnection>, NpgIndexesGetter>()
                ;
        }

    }
}
