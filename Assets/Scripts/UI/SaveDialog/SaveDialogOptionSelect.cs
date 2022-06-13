using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class SaveDialogOptionSelect : UIAction
{
    public SaveDialog.SaveDialogOption Option;
    private SaveDialog _saveDialog;

    public override IEnumerable<IEnumerable<Action>> DoAction()
    {
        _saveDialog.DialogOption = Option;
        yield break;
    }

    public override void OnSelect(bool manual)
    {
    }

    public override void OnDeselect()
    {
    }

    public override void OnInit()
    {
        _saveDialog = SaveDialog.Instance();
    }
}
