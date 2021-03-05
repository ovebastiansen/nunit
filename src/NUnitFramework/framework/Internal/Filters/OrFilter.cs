// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters
{
    /// <summary>
    /// Combines multiple filters so that a test must pass one
    /// of them in order to pass this filter.
    /// </summary>
    internal class OrFilter : CompositeFilter
    {
        /// <summary>
        /// Constructs an empty OrFilter
        /// </summary>
        public OrFilter() { }

        /// <summary>
        /// Constructs an OrFilter from an array of filters
        /// </summary>
        /// <param name="filters"></param>
        public OrFilter( params TestFilter[] filters ) : base(filters) { }

        /// <summary>
        /// Checks whether the OrFilter is matched by a test
        /// </summary>
        /// <param name="test">The test to be matched</param>
        /// <param name="negated">If set to <see langword="true"/> we are carrying a negation through</param>
        /// <returns>True if any of the component filters pass, otherwise false</returns>
        public override bool Pass( ITest test, bool negated )
        {
            if (negated)
                return Filters.All(f => f.Pass(test, negated));

            return Filters.Any(f => f.Pass(test, negated));
        }

        /// <summary>
        /// Checks whether the OrFilter is matched by a test
        /// </summary>
        /// <param name="test">The test to be matched</param>
        /// <returns>True if any of the component filters match, otherwise false</returns>
        public override bool Match( ITest test )
        {
            foreach( TestFilter filter in Filters )
                if ( filter.Match( test ) )
                    return true;

            return false;
        }

        /// <summary>
        /// Checks whether the OrFilter is explicit matched by a test
        /// </summary>
        /// <param name="test">The test to be matched</param>
        /// <returns>True if any of the component filters explicit match, otherwise false</returns>
        public override bool IsExplicitMatch( ITest test )
        {
            foreach( TestFilter filter in Filters )
                if ( filter.IsExplicitMatch( test ) )
                    return true;

            return false;
        }

        /// <summary>
        /// Gets the element name
        /// </summary>
        /// <value>Element name</value>
        protected override string ElementName
        {
            get { return "or"; }
        }

        public TestFilter Reduce()
        {
            HashSet<string> values = null;
            foreach (TestFilter filter in Filters)
            {
                if (filter is FullNameFilter { IsRegex: false } fullNameFilter)
                {
                    values ??= new HashSet<string>();
                    values.Add(fullNameFilter.ExpectedValue);
                }
                else
                {
                    return this;
                }
            }

            if (values != null)
            {
                return new OrFullNameFilter(values);
            }

            return this;
        }
    }
}
