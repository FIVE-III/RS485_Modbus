
using System.Collections.Generic;
using WpfApp1.ChildVM;
namespace WpfApp1.Service
{
    public class WindowService : IWindowService
    {
       private static CildWindowVM cildWindowVM ;
       private static AddressWindowVM addWindowVM;
        private static ReadTimeVM readTimeVM ;
        /// <summary>
        /// 返回端口数据
        /// </summary>
        /// <returns></returns>
        public List<string> PortData()
        {
            if (cildWindowVM == null)
            {
                cildWindowVM = new CildWindowVM();
                return cildWindowVM.Portdata();
            }
            return cildWindowVM.Portdata();
        }

        /// <summary>
        /// 返回地址
        /// </summary>
        /// <returns></returns>
        public string AddressId()
        {
            if (addWindowVM == null)
            {
                addWindowVM = new AddressWindowVM();
                return addWindowVM.Address;
            }
            return addWindowVM.Address;
        }

        public string Keys()
        {
           
            if (readTimeVM == null)
            {
                readTimeVM = new ReadTimeVM();
                return readTimeVM.Selectedkey;
            }
            return readTimeVM.Selectedkey;

        }
    }
}
