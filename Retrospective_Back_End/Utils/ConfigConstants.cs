﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Retrospective_Back_End.Utils
{
    public static class ConfigConstants
    {
        public static string SendGridKey = "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";
        public static string ServiceEmail = "retrospectivetool_noreply@truelime.com";
        public static string ServiceName = "Truelime Account Recovery Service";
        public static string Subject = "Password recovery";

        public static string DescriptionDutch =
            "U heeft een wachtwoordwijziging aangevraagd via de Truelime Retrospective Tool website. Als u dit niet aangevraagd heeft, negeer dan dit bericht. \bKlik op de link om uw huidige wachtwoord te wijzigen: ";


    }
}
