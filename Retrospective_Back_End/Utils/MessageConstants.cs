namespace Retrospective_Back_End.Utils
{
    public static class MessageConstants
    {
        public const string RetroUserErrorPassword =
            "Password is not valid! (min. length is 8, 1 uppercase character and 1 alphanumericals)";

        public const string AccountRecoveryOk = "A email has been sended to the account!";
        public const string AccountRecoveryBad = "Could not find user!";

        public const string ResetPasswordOk = "Password has been successfully updated!";
        public const string ResetPasswordError = "Password could not be updated! " + RetroUserErrorPassword;
        public const string ResetPasswordBad = "Could not update password (could not find user)!";

        public const string UpdatePasswordRequired = "New Password is required to update account!";
    }
}
;