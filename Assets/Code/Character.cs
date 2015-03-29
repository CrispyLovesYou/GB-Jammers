using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

public struct Character
{
    #region Fields

    public string Name;
    public string RealName;

    public float ThrowPower;
    public float ThrowKnockback;
    public float Stability;

    public float MoveSpeed;
    public float DashSpeed;
    public float DashDuration;

    public float LobDuration;

    public string AbilityPassive;
    public string AbilityEX;
    public string AbilitySuper;

    #endregion

    #region Serialization

    private static XmlSerializer serializer = new XmlSerializer(typeof(Character));

    public string Serialize()
    {
        StringBuilder builder = new StringBuilder();

        serializer.Serialize(System.Xml.XmlWriter.Create(builder), this);

        return builder.ToString();
    }

    public void SaveToXmlFile(string _filePath)
    {
        XmlSerializer writer = new XmlSerializer(typeof(Character));
        StreamWriter file = new StreamWriter(_filePath);

        try
        {
            writer.Serialize(file, this);
        }
        finally
        {
            file.Close();
        }
    }

    public static Character Deserialize(string _data)
    {
        return (Character)serializer.Deserialize(new StringReader(_data));
    }

    #endregion
}