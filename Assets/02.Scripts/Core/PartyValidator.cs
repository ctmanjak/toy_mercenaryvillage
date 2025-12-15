using System.Linq;

namespace Core
{
    public static class PartyValidator
    {
        public static bool HasAnyMember()
        {
            if (PartyManager.Instance == null) return false;
            return PartyManager.Instance.GetParty().Any(m => m != null);
        }
        
        public static int GetMemberCount()
        {
            if (PartyManager.Instance == null) return 0;
            return PartyManager.Instance.GetParty().Count(m => m != null);
        }
        
        public static bool CanStartBattle()
        {
            return HasAnyMember();
        }
        
        public static bool CanReturnToTown()
        {
            return HasAnyMember();
        }
    }
}
