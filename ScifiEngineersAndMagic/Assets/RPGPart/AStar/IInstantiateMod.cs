public interface IInstantiateMod {
    void OnBegin(params object[] data);

    void OnDone(params object[] data);

    void OnInstantiate(params object[] data);
}
