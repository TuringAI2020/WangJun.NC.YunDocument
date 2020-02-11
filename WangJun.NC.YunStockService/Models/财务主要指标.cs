using System;
using System.Collections.Generic;

namespace WangJun.NC.YunStockService.Models
{
    public partial class 财务主要指标
    {
        public string Code { get; set; }
        public int DateTag { get; set; }
        public decimal? 基本每股收益 { get; set; }
        public decimal? 扣非每股收益 { get; set; }
        public decimal? 稀释每股收益 { get; set; }
        public decimal? 每股公积金 { get; set; }
        public decimal? 每股未分配利润 { get; set; }
        public decimal? 每股经营现金流 { get; set; }
        public decimal? 营业总收入 { get; set; }
        public decimal? 毛利润 { get; set; }
        public decimal? 归属净利润 { get; set; }
        public decimal? 扣非净利润 { get; set; }
        public decimal? 营业总收入同比增长 { get; set; }
        public decimal? 归属净利润同比增长 { get; set; }
        public decimal? 扣非净利润同比增长 { get; set; }
        public decimal? 营业总收入滚动环比增长 { get; set; }
        public decimal? 归属净利润滚动环比增长 { get; set; }
        public decimal? 扣非净利润滚动环比增长 { get; set; }
        public decimal? 加权净资产收益率 { get; set; }
        public decimal? 摊薄净资产收益率 { get; set; }
        public decimal? 摊薄总资产收益率 { get; set; }
        public decimal? 毛利率 { get; set; }
        public decimal? 净利率 { get; set; }
        public decimal? 实际税率 { get; set; }
        public decimal? 预收款营业收入 { get; set; }
        public decimal? 销售现金流营业收入 { get; set; }
        public decimal? 经营现金流营业收入 { get; set; }
        public decimal? 总资产周转率 { get; set; }
        public decimal? 应收账款周转天数 { get; set; }
        public decimal? 存货周转天数 { get; set; }
        public decimal? 资产负债率 { get; set; }
        public decimal? 流动负债总负债 { get; set; }
        public decimal? 流动比率 { get; set; }
        public decimal? 速动比率 { get; set; }
    }
}
