using BTD_Mod_Helper.Api.Commands;

namespace BloonFactory.Commands;

public class BloonFactoryCommand : ModCommand
{
    public override string Command => "bloonfactory";
    public override string Help => "Admin commands for bloon factory";

    public override bool Execute(ref string arguments)
    {
        #if DEBUG 
            return ExplainSubcommands(out arguments);
        #endif
        return false;
    }
}