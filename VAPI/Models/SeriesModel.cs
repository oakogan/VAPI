using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VAPI.Models
{
    public class SeriesModel
    {
        public string SeriesName { get; set; }
        public List<Year> Years { get; set; }
    }

    public class Year
    {
        public string Number { get; set; }
        public List<Feature> Features {get; set;}
        public List<Trim> Trims { get; set; }
    }

    public class Feature
    {
        public string Name { get; set; }
        public string Guid { get; set; }
        public List<Subsection> Subsections { get; set; }
    }

    public class Subsection
    {
        public string Name { get; set; }
        public List<Spec> Specs { get; set; }
    }

    public class Spec
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Guid { get; set; }
    }

    public class Trim
    {
        public string Katashiki { get; set; }
        public string KatashikiCode { get; set; }
        public string ModelNumber { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string MatrixGuid { get; set; }
        public List<Spec> Specs { get; set; }
    }
}