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
using MattRaffelNetCode.Apps.SqlCodeGen.Classes;
#endregion

namespace AlternateMemberFormatter
{
    /// <summary>
    /// memember name formatter example 
    /// format:  m_MemberName
    /// 
    /// It looks up any subsitution values from the subsitution column and uses the definition column
    /// name should no substitution exist
    /// </summary>
    public class MUnderscoreMemberNameFormatter : IDataMemberNameFormatter
    {
        #region IDataMemberNameFormatter Members

        public string MakeDataMemberName(ColumnDefinition definition, ColumnSubstitution substitution)
        {
            string ret = definition.Name;

            // generate the Property name using override values from the default.names data
            if (null != substitution)
            {
                ret = substitution.DataMemberName;
            }

            ret = string.Format("m_{0}", ret);

            return ret;
        }

        #endregion
    }
}