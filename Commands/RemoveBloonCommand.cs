using System.Threading.Tasks;
using BloonFactory.Handlers;
using BTD_Mod_Helper.Api.Commands;
using CommandLine;
using MelonLoader;

namespace BloonFactory.Commands;

public class RemoveBloonCommand : ModCommand<BloonFactoryCommand>
{
    public override string Command => "removebloon";
    public override string Help => "Removes a bloon from the bloon browser";

    [Value(0, MetaName = "Template UUID", Required = true, HelpText = "The template to modify")]
    public string TemplateUUID { get; set;}

    [Option('a', "all", HelpText = "Removes all bloons by this user from the browser", Default = false)] 
    public bool AllByUser { get; set; }
    
    public override bool Execute(ref string resultText)
    {
        if (!AdminHandler.AdminModeEnabled)
        {
            resultText = "Admin Mode is disabled.";
            return false;
        }

        Task.Run(async () =>
        {
            var result = await ServerHandler.DeleteTemplate(new ServerHandler.DeleteRequest()
            {
                loginKey = AdminHandler.LoginKey,
                templateUUID = TemplateUUID,
                allByUser = AllByUser
            });
            
            if (!result.success)
                MelonLogger.Error(result.errorCode);
            else
                MelonLogger.Msg("Successfully removed the bloon!");
        });
        return true;
    }
}