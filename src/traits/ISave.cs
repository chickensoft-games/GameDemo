namespace GameDemo;

public interface ISave<TDataType> {
  string SaveId { get; }
  TDataType GetSaveData();
  void RestoreSaveData(TDataType data);
}
