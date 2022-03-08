/*
 ******************************************************************************
 This file is part of MattRaffelNetCode.

    MattRaffelNetCode is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    MattRaffelNetCode is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MattRaffelNetCode; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA


    architected and written by 
    matt raffel 
    matt.raffel@mindspring.com

       copyright (c) 2007 by matt raffel unless noted otherwise

 ******************************************************************************
*/
#region using statements
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using CLIENT = System.Data.SqlClient;
using MattRaffelNetCode.Apps.SqlCodeGen.Interfaces;
using MattRaffelNetCode.Apps.SqlCodeGen.Classes;
#endregion

namespace MattRaffelNetCode.Apps.SqlCodeGen
{
    class Program
    {        
        #region private data
        private ProgramConfiguration _config = ProgramConfiguration.Instance;
        private string[] _args = null;
        #endregion
       
        #region private Methods
        /// <summary>
        /// Rather than have a bunch of catch statements we will use one single catch that will call this 
        /// method will decide what we should do about an exception
        /// </summary>
        /// <param name="ex"></param>
        private void ProcessException(Exception ex)
        {
            if ((ex is BigWoo.NET.ApplicationSupport.CommandlineException)
               || (ex is HelpException)
               || (ex is InvalidCommandLineException))
            {
                PrintHelp();
            }
            else
            {
                System.Text.StringBuilder msg = new StringBuilder();
                msg.Append("Error occurred:");
                msg.Append(ex.Message);
                msg.AppendLine();

#if DEBUG
                msg.AppendFormat("error found in {0}", ex.TargetSite);
                msg.AppendLine();
#endif
                Console.Write(msg.ToString());
                Console.WriteLine("");

            }
        }

        private void PrintHelp()
        {
            
            Console.WriteLine("");

            foreach (string line in _config.CmdLine.HelpLines)
            {
                Console.WriteLine(line);
            }

            Console.WriteLine("");
            Console.WriteLine("Please note: arguments are case sensitive.");

        }

        private void PrintHeader()
        {
            Console.WriteLine("SqlCodeGen (c) 2007 mattraffel.com");
        }
        /// <summary>
        /// </summary>
        public void ParseCmdLine()
        {
            _config.ConfigFromCommandLine(_args);
        }
        #endregion

        #region public methods
        /// <summary>
        /// High level control loop
        /// </summary>
        public void Go()
        {
            try
            {
                PrintHeader();

                // defaults are already setup at this point so 
                // process command line inputs to override any of the defaults
                ParseCmdLine();

                // not going to trap for any exceptions here as what will be thrown both in TableInquiryLoader.LoadInquiry
                // and by the CLR itself is more than sufficient to catch any problems
                ITableInquiry inquiry = TableInquiryLoader.LoadInquiry(ProgramConfiguration.Instance.TableInquiryConfigString, true);

                // for each table figure out its columns
                inquiry.ConnectionStr = ProgramConfiguration.Instance.ConnectionStr;
                inquiry.ProcessTables(ProgramConfiguration.Instance.Tables);

                // and generate a code file
                CodeGeneratorController generator = new CodeGeneratorController();
                generator.ProcessTables();
            }
            catch (OutOfMemoryException)
            {
                // outta memory bad, just try to end as graceful as possible
                // otherwise do more processing on the exception
                Console.WriteLine("memory problems prevent further processing");
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debugger.Break();
                System.Diagnostics.Debug.WriteLine(ex.Message);
#endif
                ProcessException(ex);
            }
            finally
            {

            }
        }
        #endregion

        #region ctor/init/cleanup
        public Program(string[] args)
        {
            _args = args;
        }
        #endregion

        #region application start up point
        static void Main(string[] args)
        {
            Program exe = new Program(args);            
            exe.Go();
        } 
        #endregion
    }
}
