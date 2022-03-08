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
#region using statments
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MattRaffelNetCode.Apps.SqlCodeGen.Properties;
using System.Configuration;
#endregion


namespace MattRaffelNetCode.Apps.SqlCodeGen
{
    #region Snippet data class
    /// <summary>
    /// contains information about a snippet. related to SnippetElement but is not used
    /// for accessing config file like SnippetElement is used.
    /// </summary>
    internal class SnippetData : BaseData<SnippetData>
    {
        #region private data
        #endregion

        #region properties
        #endregion

        #region ctor/init/cleanup
        public SnippetData() { }

        /// <summary>
        /// factory method for creating an instance of SnippetData from SnippetElement
        /// 
        /// fyi: no difference really from SnippetElement and SnippetData.  Just didnt
        /// like using SnippetElement for non config file functions
        /// </summary>
        /// <param name="element"></param>
        /// <returns>SnippetData</returns>
        public static SnippetData FromTemplateElement(SnippetElement element)
        {
            SnippetData ret = new SnippetData();

            ret.Name = element.Name;
            ret.FileName = element.FileName;

            return ret;
        }
        #endregion

        #region public methods
        #endregion

    }
    #endregion

    #region Snippets collection implementation
    /// <summary>
    /// 
    /// </summary>
    [System.Xml.Serialization.XmlRoot("Snippets")]
    internal class SnippetDataCollection : System.Collections.Generic.List<SnippetData>
    {
        #region public methods
        /// <summary>
        /// Looks for a snippet by name.  Current a snippet name is hard coded into the application
        /// and is not user definable (user can change the content of the snippet and change the file name)
        /// </summary>
        /// <param name="name"></param>
        /// <returns>SnippetData</returns>
        public SnippetData GetByName(string name)
        {
            SnippetData ret = null;

            foreach (SnippetData snippet in this)
            {
                if (0 == snippet.Name.CompareTo(name))
                {
                    ret = snippet;
                    break;
                }
            }

            return ret;
        }
        #endregion
    }
    #endregion
}
