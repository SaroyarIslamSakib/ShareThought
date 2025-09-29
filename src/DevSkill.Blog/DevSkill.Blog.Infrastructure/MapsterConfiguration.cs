using DevSkill.Blog.Application.Features.Blogs.Commands;
using DevSkill.Blog.Domain.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSkill.Blog.Infrastructure
{
    public class MapsterConfiguration : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<BlogPostAddCommand, BlogPost>();
        }
    }
}
