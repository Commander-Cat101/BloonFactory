using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BloonFactory.Handlers;
using BTD_Mod_Helper.Api.Commands;
using BTD_Mod_Helper.Extensions;
using CommandLine;
using MelonLoader;

namespace BloonFactory.Commands;

public class RateBloonCommand : ModCommand<BloonFactoryCommand>
{
    public override string Command => "rate";
    public override string Help => "Sets a bloons verification status on the bloon browser";
    
    [Value(0, MetaName = "Template UUID", Required = true, HelpText = "The template to modify")]
    public string TemplateUUID { get; set; }
    
    [Value(1, MetaName = "Rating", Required = true, HelpText = "The rating of the bloon")]
    public string Status { get; set; }

    public override IEnumerable<string> SuggestionsForValue(int index)
    {
        if (index == 1)
        {
            return Enum.GetNames(typeof(VerificationStatus));
        }
        return new List<string>();
    }

    public override bool Execute(ref string resultText)
    {
        if (!AdminHandler.AdminModeEnabled)
        {
            resultText = "Admin Mode is disabled.";
            return false;
        }
        if (!VerificationStatus.TryParse(Status, out VerificationStatus status))
        {
            resultText = "Unknown status.";
            return false;
        }

        Task.Run(async () =>
        {
            var result = await ServerHandler.RateTemplate(new ServerHandler.RateRequest()
            {
                loginKey = AdminHandler.LoginKey,
                templateUUID = TemplateUUID,
                verificationStatus = (int)status
            });
            
            if (!result.success)
                MelonLogger.Error(result.errorCode);
            else
                MelonLogger.Msg("Successfully rated the bloon!");
        });
        return true;
    }
}