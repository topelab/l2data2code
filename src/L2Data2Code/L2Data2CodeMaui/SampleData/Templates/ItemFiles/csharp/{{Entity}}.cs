using System;
using System.Collections.Generic;
using {{Area}}.{{Module}}.Domain.Interfaces;
using {{Area}}.{{Module}}.Domain.Collections;

namespace {{Area}}.{{Module}}.Domain.Entities
{
    /// <summary>
    /// {{#Description.NotEmpty}}{{Description.HtmlEncode}}{{/Description.NotEmpty}}{{^Description.NotEmpty}}Implementation for {{Entity.Humanize}}{{/Description.NotEmpty}}
    /// </summary>
    public partial class {{Entity}}
    {
        {{#GenerateReferences}}
        {{#HasCollections}}
        /// <summary>
        /// Constructor for {{Entity.Humanize}}
        /// </summary>
        public {{Entity}}()
        {
            {{#Collections}}
            this.{{Name}} = new {{Type}}Collection();
            {{/Collections}}
        }

        {{/HasCollections}}
        {{/GenerateReferences}}
        /* --------------
        * Attributes
        * --------------*/
        {{#Columns}}
        /// <summary>
        /// {{#Description.NotEmpty}}{{Description.HtmlEncode}}{{/Description.NotEmpty}}{{^Description.NotEmpty}}{{Name.Humanize}}{{/Description.NotEmpty}}
        /// </summary>
        public {{Type}} {{Name}} { get; set; }
        {{/Columns}}
        {{#GenerateReferences}}
        {{#HasForeignKeys}}
        {{#ForeignKeyColumns}}

        /* --------------
        * FK entities
        * --------------*/
        /// <summary>
        /// {{#Description.NotEmpty}}{{Description.HtmlEncode}}{{/Description.NotEmpty}}{{^Description.NotEmpty}}{{Name.Humanize}}{{/Description.NotEmpty}}
        /// </summary>
        public {{Type}} {{Name}} { get; set; }
        {{/ForeignKeyColumns}}
        {{/HasForeignKeys}}
        {{#HasCollections}}

        /* -----------------------------------------------------------
        * Collections (othes fields from other tables linking this table)
        * -----------------------------------------------------------*/
        {{#Collections}}

        /// <summary>
        /// {{#Description.NotEmpty}}{{Description.HtmlEncode}}{{/Description.NotEmpty}}{{^Description.NotEmpty}}{{Name.Humanize}}{{/Description.NotEmpty}}
        /// </summary>
        public virtual {{Type}}Collection {{Name}} { get; set; }
        {{/Collections}}
        {{/HasCollections}}
        {{/GenerateReferences}}
    }
}
