using System.Xml.Serialization;

namespace VideoScraping.DTO.NFO;

[XmlRoot(ElementName = "season")]
public class SeasonDetailNFO
{
    [XmlElement(ElementName = "plot")]
    public object Plot { get; set; }

    [XmlElement(ElementName = "outline")]
    public object Outline { get; set; }

    [XmlElement(ElementName = "lockdata")]
    public bool Lockdata { get; set; }

    [XmlElement(ElementName = "dateadded")]
    public string DateAdded { get; set; }

    [XmlElement(ElementName = "title")]
    public string Title { get; set; }

    [XmlElement(ElementName = "year")]
    public string Year { get; set; }

    [XmlElement(ElementName = "sorttitle")]
    public string Sorttitle { get; set; }

    [XmlElement(ElementName = "premiered")]
    public string Premiered { get; set; }

    [XmlElement(ElementName = "releasedate")]
    public string ReleaseDate { get; set; }

    [XmlElement(ElementName = "seasonnumber")]
    public int SeasonNumber { get; set; }
    
    [XmlElement(ElementName = "overview")]
    public string Overview { get; set; }
}