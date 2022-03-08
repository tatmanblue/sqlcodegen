/*
 ******************************************************************************
 This file is part of BigWoo.

    BigWoo is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    BigWoo is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with BigWoo; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA


    architected and written by 
    matt raffel 
    matt.raffel@gmail.com

       copyright (c) 2010 by matt raffel unless noted otherwise

 ******************************************************************************
*/
#region using statements
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using BigWoo.Common;
using BigWoo.Apps.SqlCodeGen.Classes;
using BigWoo.Apps.SqlCodeGen;
#endregion

namespace BigWoo.Apps.SqlCodeGen
{
    /// <summary>
    /// Contains all the information that configures how the program runs
    /// 
    /// This class is global to the application.  So that we dont have to
    /// keep passing instances of it around, make the class instance
    /// accessible through a static Instance member
    /// 
    /// Data can come from/built from several places: (hopefully) in this order
    /// 1) hardcoded defaults
    /// 2) config file
    /// 3) command line input
    /// 4) changes in application as it runs
    /// </summary>
    internal class ProgramConfiguration
    {
        #region static data
        private static ProgramConfiguration _this = new ProgramConfiguration();
        #endregion

        #region data constants
        private const string DEF_DATA_SOURCE        = "SQL_SERVER";
        private const string DEF_INIT_CATALOG       = "SQL_DB";
        private const string CONNECTION_FORMAT      = "Data Source={0};Initial Catalog={1};Integrated Security=True";
        #endregion

        #region enums
        private enum WordCommandLineArgsType
        {
            Help,
            Tables,
            ClassTemplate,
            Server,
            Database,
            Namespace
        }
        #endregion

        #region private data
        #region functional data
        private ApplicationCommandLine _cmdProcessor;
        private string _connectionStr = string.Empty;
        private System.Data.SqlClient.SqlConnection _connection = null;
        #endregion

        #region configuration file driven data
        private ClassProperty _templatePath = new ClassProperty(string.Empty, "TemplatePath");
        private ClassProperty _defaultDataTypesFile = new ClassProperty(string.Empty, "DefaultDataTypesFile");
        private ClassProperty _connectionFormat = new ClassProperty(CONNECTION_FORMAT, "ConnectionFormat");
        private ClassProperty _outputPath = new ClassProperty(@".\", "OutputPath");
        private ClassProperty _sqlNamesChart = new ClassProperty(string.Empty, "DefaultSQLNameChart");
        private ClassProperty _memberNameFormatter = new ClassProperty(string.Empty, "MemberNameFormatter", true);
        private ClassProperty _tableInquiry = new ClassProperty(string.Empty, "TableInquiry", true);
        #endregion

        #region commandline driven data
        /// <summary>
        /// The server
        /// </summary>
        private ClassProperty         _dataSource = new ClassProperty(DEF_DATA_SOURCE, "Server");            
        /// <summary>
        ///  the database
        /// </summary>
        private ClassProperty         _catalog = new ClassProperty(DEF_INIT_CATALOG, "Database");            
        /// <summary>
        /// the tables to generate code
        /// </summary>
        private TableDefinitionCollection _tables = new TableDefinitionCollection();
        /// <summary>
        /// is the command line information sufficent to proceed with code gen
        /// </summary>
        private bool                    _configured;
        /// <summary>
        /// name of the class template to use for code gen
        /// </summary>
        private string                  _templateName = string.Empty;
        /// <summary>
        /// for the code gen, allows the user to specify the name space
        /// so that the template can generic for more than 1 name space
        /// </summary>
        private ClassProperty         _namespace = new ClassProperty(string.Empty, "DefaultNameSpace");          
        #endregion
        #endregion

        #region properties
        /// <summary>
        /// 
        /// </summary>
        public static ProgramConfiguration Instance
        {
            get { return _this; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string TableInquiryConfigString
        {
            get { return GetTableInquiryConfigString(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string MemberNameFormatterConfigString
        {
            get { return GetMemberNameFormatterConfigString(); }
        }

        /// <summary>
        /// path, either full or relative to the EXE path, to the directory that
        /// contains the templates used for code generation (Read Only)
        /// </summary>
        public string TemplatePath
        {
            get { return GetTemplatePath(); }
        }

        /// <summary>
        /// path, either full or relative to the EXE path, to the file that contains the
        /// default SQL to C# data type conversion information.  The DefaultDataTypesFile
        /// is XML containing entries which map a SQL Server column type to a cooresponding
        /// C# type.  
        /// 
        /// The SQL type must match (case ignorant) to values in the name field from
        /// the sys.types table.  
        /// 
        /// The C# type must match (case-sensitive) to types in the C# namespace. 
        /// 
        /// Changes to the default file affect every code generated for every table. Table specific
        /// overrides can be applied via {TODO}
        /// </summary>
        public string DefaultDataTypesFile
        {
            get { return GetDefaultDataTypesFile(); }
        }

        /// <summary>
        /// Path to the file that contains SQL column to CS data member names conversion data.  
        /// </summary>
        public string DefaultNamesChart
        {
            get { return GetDefaultSqlToCSConverionChart(); }
        }

        /// <summary>
        /// Default path for all output
        /// </summary>
        public string OutputPath
        {
            get { return GetDefaultOutputPath(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ConnectionFormat
        {
            get { return GetDefaultConnectionFormat(); }
        }

        public string ConnectionStr
        {
            get { return GetConnectionStr(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public TableDefinitionCollection Tables
        {
            get { return _tables; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string NameSpace
        {
            get { return GetDefaultNameSpace(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationCommandLine CmdLine
        {
            get { return _cmdProcessor; }
        }
        #endregion

        #region property accessor methods
        private string GetTableInquiryConfigString()
        {
            return (string)_tableInquiry.Value;
        }

        private string GetMemberNameFormatterConfigString()
        {
            return (string)_memberNameFormatter.Value;
        }

        private string GetConnectionStr()
        {
            if (true == string.IsNullOrEmpty(_connectionStr))
                _connectionStr = string.Format(_connectionFormat.Value.ToString(), _dataSource, _catalog);

            return _connectionStr;
        }

        private System.Data.SqlClient.SqlConnection GetConnection()
        {
            if (null == _connection)
            {
                _connection = new System.Data.SqlClient.SqlConnection(ConnectionStr);
            }

            return _connection;
        }

        private string GetTemplatePath()
        {
            string ret = (string)_templatePath.Value;

            if (false == ret.EndsWith(@"\"))
                ret = ret + @"\";

            return ret;

        }

        private string GetDefaultSqlToCSConverionChart()
        {
            return (string) _sqlNamesChart.Value;
        }

        private string GetDefaultDataTypesFile()
        {
            return (string) _defaultDataTypesFile.Value;
        }

        private string GetDefaultConnectionFormat()
        {
            return (string) _connectionFormat.Value;
        }

        private string GetDefaultOutputPath()
        {
            string ret = (string)_outputPath.Value;

            if (false == ret.EndsWith(@"\"))
                ret = ret + @"\";

            if (false == Directory.Exists(ret))
                Directory.CreateDirectory(ret);

            return ret;
        }

        private string GetDefaultNameSpace()
        {
            return (string) _namespace.Value;
        }
        #endregion

        #region command line processing methods
        /// <summary>
        /// simply establishes the command line argument processing class
        /// </summary>
        private void InitCommandLineProcessor()
        {
            if (null == _cmdProcessor)
            {
                _cmdProcessor = new ApplicationCommandLine();

                InitializeArguments();
            }
        }

        /// <summary>
        /// generates a set of CommandLineArguments and get them added to the hashtable
        /// </summary>
        public void InitializeArguments()
        {
            // create the help argument
            _cmdProcessor.AddArg(new CommandLineArgument("?", "prints help"));
            _cmdProcessor.AddArg(new CommandLineArgument("help", "prints help", false, WordCommandLineArgsType.Help));

            // specify tables two ways -t or -tables
            // -t format is a comma delimited string of table name, tamplate name and (optionally) output name
            // -tables is a comma delimited list of tables, --baseclasstemplate is required
            //   (output will be in the format of tablename.cs)
            _cmdProcessor.AddArg(new DataCommandLineArgument("t", "information to process one table.  \r\n\t\t\tformat: tablename,baseclass[,outputfile]\r\n\t\t\t(required, unless -table is used)"));
            _cmdProcessor.AddArg(new DataCommandLineArgument("tables", "tables for processing. \r\n\t\t\tformat: tablename[,tablename]\r\n\t\t\t(required, unless -t is used)", WordCommandLineArgsType.Tables));

            // required when used with the -tables argument, used to indicate the template file name
            _cmdProcessor.AddArg(new DataCommandLineArgument("classtemplate", "template name, used for class code gen (required)", WordCommandLineArgsType.ClassTemplate));

            // sql server name (optional, overrides config file)
            _cmdProcessor.AddArg(new DataCommandLineArgument("s", "server name"));
            _cmdProcessor.AddArg(new DataCommandLineArgument("server", "server name", WordCommandLineArgsType.Server));

            // sql server name (optional, overrides config file)
            _cmdProcessor.AddArg(new DataCommandLineArgument("n", "namespace for the code gen files"));
            _cmdProcessor.AddArg(new DataCommandLineArgument("namespace", "namespace for the code gen files", WordCommandLineArgsType.Namespace));

            // the database name (optional, overrides config file)
            _cmdProcessor.AddArg(new DataCommandLineArgument("d", "database name"));
            _cmdProcessor.AddArg(new DataCommandLineArgument("database", "database name", WordCommandLineArgsType.Database));

        }

        /// <summary>
        /// going through the commandline arguments, setup the program data according to the input
        /// </summary>
        private void ArgsToProgramData()
        {
            foreach (CommandLineArgument argument in _cmdProcessor)
            {
                if (false == argument.Selected)
                    continue;

                if (1 == argument.Argument.Length)
                {
                    HandleOldStyleArgument(argument);
                }
                else
                {
                    HandleLongWordStyleArgument(argument);
                }
            }

            CheckConfigured();
            FinalizeData();
        }

        /// <summary>
        /// handles the "old style" single charactor form of command line arguments
        /// </summary>
        /// <param name="argument">CommandLineArgument</param>
        private void HandleOldStyleArgument(CommandLineArgument argument)
        {
            DataCommandLineArgument dataArg = (argument as DataCommandLineArgument);

            switch (argument.Argument[0])
            {
                case 't':
                    ConvertArgToTable(dataArg.Data);
                    // going to make an assmption that the data passed in using the -t commandline 
                    // was valid and a table was made with a template. if the data is invalid the
                    // TableDefinition factory method will throw an exception
                    _configured = true;
                    break;
                case 's':
                    _dataSource.Value = dataArg.Data;
                    break;
                case 'd':
                    _catalog.Value = dataArg.Data;
                    break;
                case 'n':
                    _namespace.Value = dataArg.Data;
                    break;
                case '?':
                    throw new HelpException();
                default:
                    throw new InvalidCommandLineException(string.Format("{0} is not a supported argument", argument.Argument));

            }
        }

        /// <summary>
        /// handle the more recent style of command line arguments of using meaningful words
        /// </summary>
        /// <param name="argument">CommandLineArgument</param>
        private void HandleLongWordStyleArgument(CommandLineArgument argument)
        {
            DataCommandLineArgument dataArg = (argument as DataCommandLineArgument);
            WordCommandLineArgsType type = (WordCommandLineArgsType) argument.Id;

            switch (type)
            {
                case WordCommandLineArgsType.Database:
                    _catalog.Value = dataArg.Data;
                    break;
                case WordCommandLineArgsType.Server:
                    _dataSource.Value = dataArg.Data;
                    break;
                case WordCommandLineArgsType.Tables:
                    string tableName = dataArg.Data;
                    ConvertArgToPartialTable(tableName);
                    break;
                case WordCommandLineArgsType.ClassTemplate:
                    _templateName = dataArg.Data;                    
                    break;
                case WordCommandLineArgsType.Namespace:
                    _namespace.Value = dataArg.Data;
                    break;
                case WordCommandLineArgsType.Help:
                    throw new HelpException();
                default:
                    throw new InvalidCommandLineException(string.Format("{0} is not a supported argument", argument.Argument));
            }
        }

        /// <summary>
        /// makes sure all the informatin we recieved from the command line was
        /// valid and usable.  if not an exception is thrown
        /// </summary>
        private void CheckConfigured()
        {
            if (false == _configured)
            {
                if (false == string.IsNullOrEmpty(_templateName))
                {
                    if (0 < _tables.Count)
                    {
                        _configured = true;
                    }
                }
            }

            if (true == _configured)
                return;

            throw new InvalidCommandLineException("Argument input was insufficient to continue.");
        }

        /// <summary>
        /// because we are supporting "old" and "new" style command line arguments (aka -t or -tables
        /// the data might come in multiple ways so we have to make sure it is all "synced" up after its
        /// been processed.  this method finalizes that...
        /// </summary>
        private void FinalizeData()
        {
            // if we have a template name passed in the commandline via -classtemplate
            // we have to assume we got table names via the -tables argument
            // which means we need to sync up the templates to the tables since that
            // is not possible while processing either those arguments.  we don't want
            // to assume every table needs the template since it is still possible that
            // table data was input via -t argument.  So we will only assign the template
            // name to tables where the name is empty
            if (false == string.IsNullOrEmpty(_templateName))
            {
                foreach(TableDefinition table in _tables)
                {
                    if (true == string.IsNullOrEmpty(table.Template))
                        table.Template = _templateName;
                }
            }
        }

        /// <summary>
        /// helper function for -t argument inputs
        /// </summary>
        /// <param name="text">string, expected input potentially comma delimited</param>
        private void ConvertArgToTable(string text)
        {
            _tables.Add(TableDefinition.FromCmdLineArgument(text));
        }

        /// <summary>
        /// processes -table argument, could be formatted like --table table1,table2
        /// </summary>
        /// <param name="text">string, expected input potentially comma delimited</param>
        private void ConvertArgToPartialTable(string text)
        {
            string[] tableNames = text.Split(new char[] { ',' });
            foreach (string table in tableNames)
            {
                TableDefinition newTable = new TableDefinition(table);
                _tables.Add(newTable);
            }
        }
        #endregion

        #region private methods
        #endregion

        #region ctor/init/factory methods
        /// <summary>
        /// 
        /// </summary>
        private ProgramConfiguration() {}

        /// <summary>
        /// 
        /// </summary>
        static ProgramConfiguration() {}

        #endregion

        #region public methods
        /// <summary>
        /// sets program configuration based on input from command line
        /// </summary>
        /// <param name="args"></param>
        public void ConfigFromCommandLine(string[] args)
        {
            InitCommandLineProcessor();

            _cmdProcessor.ParseCmdLineToArgs(args);

            ArgsToProgramData();

        }
        #endregion
    }
}
