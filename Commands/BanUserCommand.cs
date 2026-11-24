using System.Threading.Tasks;
using BloonFactory.Handlers;
using BTD_Mod_Helper.Api.Commands;
using CommandLine;
using MelonLoader;

namespace BloonFactory.Commands;

public class BanUserCommand : ModCommand<BloonFactoryCommand>
{
    public override string Command => "ban";
    public override string Help => "Bans the creator of a given bloon or by short code";
    
    [Value(0, MetaName = "Template or Short Code", Required = true, HelpText = "The template or user short code")]
    public string TemplateUUIDOrShortCode { get; set;}
    
    public override bool Execute(ref string resultText)
    {
        if (!AdminHandler.AdminModeEnabled)
        {
            resultText = "Admin Mode is disabled.";
            return false;
        }

        Task.Run(async () =>
        {
            var result = await ServerHandler.BanTemplate(new ServerHandler.BanRequest()
            {
                loginKey = AdminHandler.LoginKey,
                templateUUIDOrShortCode = TemplateUUIDOrShortCode
            });
            
            if (!result.success)
                MelonLogger.Error(result.errorCode);
            else
                MelonLogger.Msg("Successfully removed the bloon!");
        });
        return true;
    }
}