﻿using System.Collections.Generic;

namespace Dsmviz.Datamodel.Dsm.Interfaces
{
    public interface IDsmRelation
    {
        /// <summary>
        /// Unique and non-modifiable Number identifying the relation.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The consumer element.
        /// </summary>
        IDsmElement Consumer { get; }

        /// <summary>
        /// The provider element.
        /// </summary>
        IDsmElement Provider { get; }

        /// <summary>
        /// Type of relation.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Strength or weight of the relation
        /// </summary>
        int Weight { get; }

        bool IsDeleted { get; }

        // Named properties found for this relation
        IDictionary<string, string> Properties { get; }

        // Property names found across all relations
        IEnumerable<string> DiscoveredRelationPropertyNames();
    }
}
