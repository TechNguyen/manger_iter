using It_Supporter.Models;

namespace It_Supporter.BackGroundJob
{
    public interface IbirthDay
    {
        Task<bool?> sendMailHappyBirday();
    }
}
