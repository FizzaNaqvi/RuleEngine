using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_PropertyGridPractice.NewFolder1
{
    public enum ENOrderEventType
    {
        None,
        NewOrder,
        OrderRoute,
        OrderAccepted,
        OrderModification,
        OrderCancellation,
        OrderTransfer,
        OrderExecution,
        PostTradeAllocation,
        NewOrderRoute,
        NewOrderExecution,
        OrderRouteModification,
        OrderRouteCancellation,

        NewOptionOrder,
        OptionOrderRoute,
        OptionOrderAccepted,
        OptionOrderModification,
        OptionOrderCancellation,
        OptionOrderTransfer,
        OptionOrderExecution,
        OptionPostTradeAllocation,
        OrderFulFillment,
        OptionOrderFulFillment,
        OptionOrderRouteModification,
        OptionOrderRouteCancellation,

        NewMultiLegOrder,
        MultiLegOrderRoute,
        MultiLegOrderAccepted,
        MultiLegOrderModification,
        MultiLegOrderCancellation,
        MultiLegOrderRouteModification,
        MultiLegOrderRouteCancellation,

    };

    [Flags]
    public enum ENReportType
    {
        None = 0,
        Order = 1,
        Fill = 2,
        Task = 4,
        Allocation = 8,
        ACT = 16,
        IOI = 32,
    }
}
