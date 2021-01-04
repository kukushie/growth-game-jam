using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializedHoneycombData
{
    public byte[] bytes;

    public HoneycombData Deserialize() {
        IFormatter formatter = new BinaryFormatter();  
        MemoryStream stream = new MemoryStream(this.bytes);
        return (HoneycombData) formatter.Deserialize(stream);
    }
}
