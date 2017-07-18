namespace Mantle.Web.Infrastructure
{
    public struct DurandalRoute
    {
        public string ModuleId { get; set; }

        public string Title { get; set; }

        public string Route { get; set; }

        public string JsPath { get; set; } //note: for embedded scripts, we can use a virtual path provider

        //public bool Nav { get; set; }
    }
}