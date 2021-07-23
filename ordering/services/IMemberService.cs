using ordering.models;
using System.Threading.Tasks;

namespace ordering.services
{
    public interface IMemberService
    {
        Task<MemberVM> GetMemberInfo(string id);
        MemberVM GetMemberInfoFallback(string id);
    }
}