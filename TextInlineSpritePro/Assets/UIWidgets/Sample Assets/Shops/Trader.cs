using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UIWidgets;

namespace UIWidgetsSamples.Shops {
	public delegate void OnItemsChange();
	public delegate void OnMoneyChange();

	public class Trader {
		// -1 to infinite money
		int money;

		public int Money {
			get {
				return money;
			}
			set {
				if (money==-1)
				{
					MoneyChanged();
					return ;
				}
				money = value;
				MoneyChanged();
			}
		}

		ObservableList<Item> inventory = new ObservableList<Item>();

		public ObservableList<Item> Inventory {
			get {
				return inventory;
			}
			set {
				if (inventory!=null)
				{
					inventory.OnChange -= ItemsChanged;
				}
				inventory = value;
				if (inventory!=null)
				{
					inventory.OnChange += ItemsChanged;
				}
				ItemsChanged();
			}
		}

		public float PriceFactor = 1;

		public bool DeleteIfEmpty = true;

		public event OnItemsChange OnItemsChange;

		public event OnMoneyChange OnMoneyChange;

		public Trader(bool deleteIfEmpty = true)
		{
			DeleteIfEmpty = deleteIfEmpty;
			inventory.OnChange += ItemsChanged;
		}

		void ItemsChanged()
		{
			if (OnItemsChange!=null)
			{
				OnItemsChange();
			}
		}

		void MoneyChanged()
		{
			if (OnMoneyChange!=null)
			{
				OnMoneyChange();
			}
		}

		public void Sell(IOrder order)
		{
			if (order.OrderLinesCount()==0)
			{
				return ;
			}

			Inventory.BeginUpdate();
			order.GetOrderLines().ForEach(SellItem);
			Inventory.EndUpdate();

			Money += order.Total();
		}

		void SellItem(IOrderLine orderLine)
		{
			var count = orderLine.Count;

			// decrease items count
			orderLine.Item.Count -= count;

			// remove item from inventory if zero count
			if (DeleteIfEmpty && (orderLine.Item.Count==0))
			{
				Inventory.Remove(orderLine.Item);
			}
		}
		public void Buy(IOrder order)
		{
			if (order.OrderLinesCount()==0)
			{
				return ;
			}

			Inventory.BeginUpdate();
			order.GetOrderLines().ForEach(BuyItem);
			Inventory.EndUpdate();

			Money -= order.Total();
		}

		void BuyItem(IOrderLine orderLine)
		{
			// find item in inventory
			var item = Inventory.Find(x => x.Name==orderLine.Item.Name);

			var count = orderLine.Count;
			// if not found add new item to inventory
			if (item==null)
			{
				Inventory.Add(new Item(orderLine.Item.Name, count));
			}
			// if found increase count if items count not infinite
			else
			{
				item.Count += count;
			}
		}

		public bool CanBuy(IOrder order)
		{
			return Money==-1 || Money>=order.Total();
		}
	}
}