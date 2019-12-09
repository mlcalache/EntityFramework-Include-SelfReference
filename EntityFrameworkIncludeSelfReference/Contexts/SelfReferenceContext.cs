﻿using EntityFrameworkIncludeSelfReference.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EntityFrameworkIncludeSelfReference.Contexts
{
    public class SelfReferenceContext : DbContext
    {
        public SelfReferenceContext()
            : base("name=SelfReference")
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public virtual DbSet<Activity> Activities { get; set; }
        public virtual DbSet<Document> Documents { get; set; }

        public void objContext_SavingChanges(object sender, EventArgs e)
        {
            var commandText = new StringBuilder();

            var conn = sender.GetType()
                 .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                 .Where(p => p.Name == "Connection")
                 .Select(p => p.GetValue(sender, null))
                 .SingleOrDefault();
            var entityConn = (EntityConnection)conn;

            var objStateManager = (System.Data.Entity.Core.Objects.ObjectStateManager)sender.GetType()
                  .GetProperty("ObjectStateManager", BindingFlags.Instance | BindingFlags.Public)
                  .GetValue(sender, null);

            var workspace = entityConn.GetMetadataWorkspace();

            var translatorT =
                sender.GetType().Assembly.GetType("System.Data.Entity.Core.Mapping.Update.Internal.UpdateTranslator");

            var entityAdapterT =
                sender.GetType().Assembly.GetType("System.Data.Entity.Core.EntityClient.Internal.EntityAdapter");
            var entityAdapter = Activator.CreateInstance(entityAdapterT, BindingFlags.Instance |
                BindingFlags.NonPublic | BindingFlags.Public, null, new object[] { sender }, System.Globalization.CultureInfo.InvariantCulture);

            entityAdapterT.GetProperty("Connection").SetValue(entityAdapter, entityConn);

            var translator = Activator.CreateInstance(translatorT, BindingFlags.Instance |
                BindingFlags.NonPublic | BindingFlags.Public, null, new object[] { entityAdapter }, System.Globalization.CultureInfo.InvariantCulture);

            var produceCommands = translator.GetType().GetMethod(
                "ProduceCommands", BindingFlags.NonPublic | BindingFlags.Instance);

            var commands = (IEnumerable<object>)produceCommands.Invoke(translator, null);

            foreach (var cmd in commands)
            {
                var identifierValues = new Dictionary<int, object>();
                var dcmd =
                    (System.Data.Common.DbCommand)cmd.GetType()
                       .GetMethod("CreateCommand", BindingFlags.Instance | BindingFlags.NonPublic)
                       .Invoke(cmd, new[] { identifierValues });

                foreach (System.Data.Common.DbParameter param in dcmd.Parameters)
                {
                    var sqlParam = (SqlParameter)param;

                    commandText.AppendLine(String.Format("declare {0} {1} {2}",
                                                            sqlParam.ParameterName,
                                                            sqlParam.SqlDbType.ToString().ToLower(),
                                                            sqlParam.Size > 0 ? "(" + sqlParam.Size + ")" : ""));

                    commandText.AppendLine(String.Format("set {0} = '{1}'", sqlParam.ParameterName, sqlParam.SqlValue));
                }

                commandText.AppendLine();
                commandText.AppendLine(dcmd.CommandText);
                commandText.AppendLine("go");
                commandText.AppendLine();
            }

            System.Diagnostics.Debug.Write(commandText.ToString());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Types().Configure(delegate (ConventionTypeConfiguration p)
            {
                if (p.ClrType.GetProperty("ValidationResult") != null)
                {
                    p.Ignore("ValidationResult");
                }
            });

            var typesMapping = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => !string.IsNullOrEmpty(type.Namespace))
                .Where(type => type.BaseType != null && type.BaseType.IsGenericType
                    && type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>));

            foreach (var mapping in typesMapping)
            {
                dynamic configurationInstance = Activator.CreateInstance(mapping);
                modelBuilder.Configurations.Add(configurationInstance);
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}