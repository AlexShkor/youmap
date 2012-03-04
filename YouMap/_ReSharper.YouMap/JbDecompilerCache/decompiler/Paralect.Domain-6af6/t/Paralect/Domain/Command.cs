// Type: Paralect.Domain.Command
// Assembly: Paralect.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: D:\install\YouMap\YouMapCqrsMongo\YouMap\libs\Paralect\Paralect.Domain.dll

namespace Paralect.Domain
{
  public class Command : ICommand
  {
    private ICommandMetadata _metadata = (ICommandMetadata) new CommandMetadata();

    public ICommandMetadata Metadata
    {
      get
      {
        return this._metadata;
      }
      set
      {
        this._metadata = value;
      }
    }
  }
}
