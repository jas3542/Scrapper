using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobScraper.Models
{
    class Location
    {
        [JsonProperty(PropertyName = "header")]
        public Header Header { get; set; }
        
        [JsonProperty(PropertyName = "results")]
        public Result[] Results { get; set; }

    }

    public class Header {
        [JsonProperty(PropertyName = "uri")]
        public string Uri { get; set; }
        [JsonProperty(PropertyName = "query")]
        public string Query { get; set; }
        [JsonProperty(PropertyName = "format")]
        public string Format { get; set; }
        [JsonProperty(PropertyName = "maxresults")]
        public int MaxResults { get; set; }
        [JsonProperty(PropertyName = "offset")]
        public int OffSet { get; set; }
        [JsonProperty(PropertyName = "totalresults")]
        public int TotalResults { get; set; }
    }

    public class Result
    {
        [JsonProperty(PropertyName = "GAZETTEER_ENTRY")]
        public GAZETTEER_ENTRY LocationData { get; set; }
    }

    public class GAZETTEER_ENTRY
    {
        [JsonProperty(PropertyName = "ID")]
        public string Id { get; set; }
        //[JsonProperty(PropertyName = "NAMES_URI")]
        //public string NameUri { get; set; }
        [JsonProperty(PropertyName = "NAME1")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "TYPE")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "LOCAL_TYPE")]
        public string Local_Type { get; set; }
        //[JsonProperty(PropertyName = "GEOMETRY_X")]
        //public long Geometry_x { get; set; }
        //[JsonProperty(PropertyName = "GEOMETRY_Y")]
        //public long Geometry_y { get; set; }
        //[JsonProperty(PropertyName = "MOST_DETAIL_VIEW_RES")]
        //public long Most_detail_view_res { get; set; }
        //[JsonProperty(PropertyName = "LEAST_DETAIL_VIEW_RES")]
        //public long Least_detail_view_res { get; set; }
        //[JsonProperty(PropertyName = "MBR_XMIN")]
        //public long Mbr_xMin { get; set; }
        //[JsonProperty(PropertyName = "MBR_YMIN")]
        //public long Mbr_yMin { get; set; }
        //[JsonProperty(PropertyName = "MBR_XMAX")]
        //public long Mbr_xMAX { get; set; }
        //[JsonProperty(PropertyName = "MBR_YMAX")]
        //public long Mbr_yMAX { get; set; }
        [JsonProperty(PropertyName = "POSTCODE_DISTRICT")]
        public string Postcode_district { get; set; }
        //[JsonProperty(PropertyName = "POSTCODE_DISTRICT_URI")]
        //public string Postcode_district_uri { get; set; }
        [JsonProperty(PropertyName = "DISTRICT_BOROUGH")]
        public string District_borough { get; set; }
        //[JsonProperty(PropertyName = "DISTRICT_BOROUGH_URI")]
        //public string District_borough_uri { get; set; }
        //[JsonProperty(PropertyName = "DISTRICT_BOROUGH_TYPE")]
        //public string District_borough_type { get; set; }
        //[JsonProperty(PropertyName = "COUNTY_UNITARY")]
        //public string County_unitary { get; set; }
        //[JsonProperty(PropertyName = "COUNTY_UNITARY_URI")]
        //public string County_unitary_uri { get; set; }
        //[JsonProperty(PropertyName = "COUNTY_UNITARY_TYPE")]
        //public string County_unitary_type { get; set; }
        //[JsonProperty(PropertyName = "REGION")]
        //public string Region { get; set; }
        //[JsonProperty(PropertyName = "REGION_URI")]
        //public string Region_uri { get; set; }
        [JsonProperty(PropertyName = "COUNTRY")]
        public string Country { get; set; }
        //[JsonProperty(PropertyName = "COUNTRY_URI")]
        //public string Country_uri { get; set; }
        //[JsonProperty(PropertyName = "SAME_AS_GEONAMES")]
        //public string Same_as_geonames { get; set; }
        //[JsonProperty(PropertyName = "SAME_AS_DBPEDIA")]
        //public string Same_as_DBPEDIA { get; set; }

        [JsonProperty(PropertyName = "POPULATED_PLACE")]
        public string Populated_place { get; set; }
    }




}
