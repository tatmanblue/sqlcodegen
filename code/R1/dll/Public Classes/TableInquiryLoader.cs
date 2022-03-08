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
using System.Text;
using MattRaffelNetCode.Apps.SqlCodeGen.Interfaces;
#endregion

namespace MattRaffelNetCode.Apps.SqlCodeGen.Classes
{
    #region TableInquiryLoader Class Implementation
    /// <summary>
    /// Used to load custom ITableInquiry implementation. <see cref="ITableInquiry"/>
    /// </summary>
    public class TableInquiryLoader
    {
        #region private methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configurationInfoStr"></param>
        /// <returns></returns>
        private static ITableInquiry LoadInquiry(string configurationInfoStr)
        {
            ITableInquiry inquiry = null;

            const int ASSEMBLY = 1;
            const int CLASSNAME = 0;

            // expecting configurationInfoStr to look like:
            // fullynamedspaceclassname,assemblyname 
            //
            // such as:
            // MattRaffelNetCode.Apps.SqlCodeGen.Classes.SqlTableInquiry,SqlCodeGenSupport

            string[] assemblyParts = configurationInfoStr.Split(new char[] { ',' });

            if (2 != assemblyParts.Length)
                throw new TypeLoaderException(string.Format("{0} is not valid", configurationInfoStr));

            inquiry = (ITableInquiry)AppDomain.CurrentDomain.CreateInstanceAndUnwrap(assemblyParts[ASSEMBLY], assemblyParts[CLASSNAME]);

            return inquiry;
        }
        #endregion

        #region public methods
        /// <summary>
        /// Used to load the ITableInquiry implementation specified in the config file
        /// </summary>
        /// <param name="configurationInfo">string, from the config file ApplicationSettings.Settings.TableInquiry</param>
        /// <param name="loadDefaultOnFailure">bool, if true and the specified type cannot be load will load the default instead</param>
        /// <returns>ITableInquiry instance, null if formatter could not be loaded</returns>
        public static ITableInquiry LoadInquiry(string configurationInfoStr, bool loadDefaultOnFailure)
        {
            ITableInquiry inquiry = null;

            try
            {
                inquiry = LoadInquiry(configurationInfoStr);
            }
            catch
            {
                if ((true == loadDefaultOnFailure) && (null == inquiry))
                {
                    const string DEFAULT_CONFIG_INFO = "MattRaffelNetCode.Apps.SqlCodeGen.Classes.SqlTableInquiry,SqlCodeGenSupport";
                    inquiry = LoadInquiry(DEFAULT_CONFIG_INFO);
                }
            }

            return inquiry;
        }
        #endregion
    }
    #endregion
}
