namespace CityPlannerPharaoh;

internal class UndoStack<T>
{
    private readonly T[] storageArray;
    private int startIndex;
    private int count;
    private int appliedCount;
    private int savedCount;

    public UndoStack(int capacity)
    {
        this.storageArray = new T[capacity];
        this.startIndex = 0;
        this.count = 0;
        this.appliedCount = 0;
        this.savedCount = 0;
    }

    public int Count => this.count;

    public bool CanUndo => this.appliedCount > 1;

    public bool CanRedo => this.appliedCount < this.count;

    public bool IsChanged => this.savedCount != this.appliedCount;

    public void Init(T value)
    {
        Array.Clear(this.storageArray);
        this.startIndex = 0;
        this.storageArray[0] = value;
        this.count = 1;
        this.appliedCount = 1;
        this.savedCount = 1;
    }

    /// <summary>
    /// Switches out the previous one with the new value, and move the current value forward.
    /// </summary>
    public void Do(T value)
    {
        int currentStorageIndex = (this.startIndex + this.appliedCount - 1) % this.storageArray.Length;
        int newStorageIndex = (currentStorageIndex + 1) % this.storageArray.Length;

        this.storageArray[newStorageIndex] = this.storageArray[currentStorageIndex];
        this.storageArray[currentStorageIndex] = value;

        if (this.appliedCount < this.storageArray.Length)
        {
            this.appliedCount++;
        }
        else
        {
            this.startIndex++;
        }

        // clear any remaining redo records, beyond the one we just overwrote, so we don't keep refs
        for (int i = this.appliedCount + 1; i < this.count; i++)
        {
            int clearStorageIndex = (this.startIndex + i - 1) % this.storageArray.Length;
            this.storageArray[clearStorageIndex] = default!;
        }

        this.count = this.appliedCount;
    }

    public T Undo()
    {
        if (!this.CanUndo)
        {
            throw new InvalidOperationException("Nothing to undo to.");
        }

        this.appliedCount--;
        int storageIndex = (this.startIndex + this.appliedCount - 1) % this.storageArray.Length;
        return this.storageArray[storageIndex];
    }

    public T Redo()
    {
        if (!this.CanRedo)
        {
            throw new InvalidOperationException("Nothing to redo to.");
        }

        this.appliedCount++;
        int storageIndex = (this.startIndex + this.appliedCount - 1) % this.storageArray.Length;
        return this.storageArray[storageIndex];
    }

    public void Save()
    {
        this.savedCount = this.appliedCount;
    }
}
