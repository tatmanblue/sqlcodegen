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
using System.Collections.Generic;
using System.Text;
using BigWoo.Apps.SqlCodeGen.Interfaces;
using BigWoo.Common; 
#endregion

namespace BigWoo.Apps.SqlCodeGen.Classes
{
    /// <summary>
    /// Default memember name formatter for sqlcodegen.exe formatting member names in the following
    /// format:  _memberName
    /// 
    /// It looks up any subsitution values from the subsitution column and uses the definition column
    /// name should no substitution exist
    /// </summary>
    public class DefaulMemberNameFormatter : IDataMemberNameFormatter
    {
        #region IDataMemberNameFormatter Members

        public string MakeDataMemberName(ColumnDefinition definition, ColumnSubstitution substitution)
        {
            string ret = definition.Name;

            // start by making a default
            ret = StringHelper.LowerCaseFirstChar(ret);

            // generate the Property name using override values from the default.names data
            if (null != substitution)
            {
                if (false == substitution.Exclude)
                {
                    ret = substitution.DataMemberName;
                    if (false == substitution.KeepCase)
                        ret = StringHelper.LowerCaseFirstChar(ret);

                }
            }

            ret = string.Format("_{0}", ret);

            return ret;
        }

        #endregion
    }
}