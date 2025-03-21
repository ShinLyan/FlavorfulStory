namespace FlavorfulStory.Actions
{
    /// <summary> Интерфейс съедобного предмета </summary>
    public interface IEdible
    {
        ///<summary> Съесть предмет и применить его эффект к игроку. </summary>
        void Eat();

        //На будущее
        //void Eat(PlayerStats stats);
    }
}