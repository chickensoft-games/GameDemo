namespace GameDemo;

public interface ISaveSerializer<TSaveFile> {
  string Serialize(TSaveFile saveFile);
  TSaveFile Deserialize(string fileContents);
}
