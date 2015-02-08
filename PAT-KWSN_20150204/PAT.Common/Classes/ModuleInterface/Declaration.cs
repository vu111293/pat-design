namespace PAT.Common.Classes.ModuleInterface
{
    public enum DeclarationType
    {
        Constant,
        Channel,
        Variable,
        Process,
        Aphabet,
        Declaration,
        Assertion,
    }

    public class Declaration
    {
        public DeclarationType DeclarationType;
        public ParsingException DeclarationToken;

        public Declaration(DeclarationType type, ParsingException token)
        {
            DeclarationType = type;
            DeclarationToken = token;
        }
    }
}
