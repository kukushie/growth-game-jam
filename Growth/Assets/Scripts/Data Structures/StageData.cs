using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

[System.Serializable]
public class StageData
{
    public static StageData DeserializeFromXML(string xml) {
        XmlSerializer serializer = new XmlSerializer(typeof(StageData));
        TextReader reader = new StringReader(xml);
        StageData data = (StageData) serializer.Deserialize(reader);
        reader.Close();
        return data;
    }

    public string name = ""; // Optional.
    public string instructions = ""; // Optional.
    public int growthPerSegment;
    public HoneycombData honeycomb;

    public string SerializeToXML() {
        XmlSerializer serializer = new XmlSerializer(typeof(StageData));
        TextWriter writer = new StringWriter();
        serializer.Serialize(writer, this);
        string xml = writer.ToString();
        writer.Close();
        return xml;
    }
}
