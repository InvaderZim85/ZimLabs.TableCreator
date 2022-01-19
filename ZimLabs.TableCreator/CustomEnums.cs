using System;
using System.Collections.Generic;
using System.Text;

namespace ZimLabs.TableCreator
{
    /// <summary>
    /// The different supported table types
    /// </summary>
    public enum OutputType
    {
        /// <summary>
        /// The default table type
        /// </summary>
        Default,

        /// <summary>
        /// The table as markdown
        /// </summary>
        Markdown,

        /// <summary>
        /// The table as a comma separated value list
        /// </summary>
        Csv
    }

    /// <summary>
    /// The different list types
    /// </summary>
    public enum ListType
    {
        /// <summary>
        /// Bullet list
        /// </summary>
        Bullets,

        /// <summary>
        /// Numbered list
        /// </summary>
        Numbers
    }
}
