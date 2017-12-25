using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(ProgrammingForumDbContext context) : base(context)
        {
        }
    }
}
