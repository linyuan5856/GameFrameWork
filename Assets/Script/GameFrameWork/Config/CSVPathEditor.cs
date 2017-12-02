using UnityEngine;
using System;

namespace Pandora
{
    [Serializable]
    public class CSVPathEditor : ScriptableObject
    {
        public string Path_ExcelInputDir;
    }

    [Serializable]
    public class TableConfig
    {
        public TableConfig(string tableCalssName, string rowClassName, string shortPath)
        {
            this.tableClassName = tableCalssName;
            this.rowClassName = rowClassName;
            this.shortPath = shortPath;
        }

        public string tableClassName;
        public string rowClassName;
        public string shortPath;
    }
}
