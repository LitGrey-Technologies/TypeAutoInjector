using System.ComponentModel.DataAnnotations;

namespace TypeAutoInjector.Enums
{
    public enum TypeLifeScope
    {
        [Display(Name = "Transient Type-Life Scope")]
        Transient = 0,

        [Display(Name = "Scoped Type-Life Scope")]
        Scoped = 1,

        [Display(Name = "Singleton Type-Life Scope")]
        Singleton = 2
    }
}