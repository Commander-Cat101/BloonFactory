using BloonFactory.Handlers;
using BTD_Mod_Helper.Api.Commands;
using CommandLine;

namespace BloonFactory.Commands;

public class AdminLoginCommand : ModCommand<BloonFactoryCommand>
{
    public override string Command => "login";
    public override string Help => "Enables admin mode with the given login key.";
    
    [Value(0, MetaName = "Login Key", Required = true, HelpText = "The admin login key required to do stuff.")]
    public string LoginKey { get; set; }
    
    public override bool Execute(ref string resultText)
    {
        AdminHandler.LoginKey = LoginKey;
        AdminHandler.AdminModeEnabled = LoginKey != null;
        
        resultText = $"Admin mode {(AdminHandler.AdminModeEnabled ? "Enabled" : "Disabled")}.";
        return true;
    }
}