using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class UnitOfWork
    {
        private ProgrammingForumDbContext Context = new ProgrammingForumDbContext();

        private UserRepository userRepository;
        private QuestionRepository questionRepository;
        private CommentRepository commentRepository;
        private CategoryRepository categoryRepository;

        public UserRepository UserRepository
        {
            get
            {
                if (this.userRepository == null)
                {
                    this.userRepository = new UserRepository(Context);
                }
                return userRepository;
            }
        }

        public QuestionRepository QuestionRepository
        {
            get
            {
                if (this.questionRepository == null)
                {
                    this.questionRepository = new QuestionRepository(Context);
                }
                return questionRepository;
            }
        }

        public CommentRepository CommentRepository
        {
            get
            {
                if (this.commentRepository == null)
                {
                    this.commentRepository = new CommentRepository(Context);
                }
                return commentRepository;
            }
        }

        public CategoryRepository CategoryRepository
        {
            get
            {
                if (this.categoryRepository == null)
                {
                    this.categoryRepository = new CategoryRepository(Context);
                }
                return categoryRepository;
            }
        }

        public int Save()
        {
            return Context.SaveChanges();
        }
    }
}
