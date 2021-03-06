﻿using System.Linq;
using System.Windows;
using TiTsEd.Model;

namespace TiTsEd.ViewModel {
    public sealed class StatusEffectVM : StorageClassVM {
        public StatusEffectVM(GameVM game, AmfObject statuses, XmlStorageClass xml)
            : base(game, statuses, xml) {
        }

        public override Visibility MinutesLeftVisibility {
            get { return Visibility.Visible; }
        }

        public override AmfObject GetItems() {
            return _game.Character.StatusEffectsArray;
        }

        public override AmfObject GetObject() {
            return GetItems().Select(x => x.ValueAsObject).FirstOrDefault(x => IsObject(x));
        }

        public override bool IsOwned {
            get { return GetObject() != null; }
            set {
                var items = GetItems();
                var pair = items.FirstOrDefault(x => IsObject(x.ValueAsObject));
                if ((pair != null) == value) return;

                if (value) {
                    var obj = new AmfObject(AmfTypes.Object);
                    InitializeObject(obj);
                    obj["value1"] = _xml.Value1;
                    obj["value2"] = _xml.Value2;
                    obj["value3"] = _xml.Value3;
                    obj["value4"] = _xml.Value4;
                    obj["hidden"] = _xml.IsHidden;
                    //obj["combatOnly"] = _xml.IsCombatOnly;
                    obj["minutesLeft"] = _xml.MinutesLeft;
                    obj["iconName"] = _xml.IconName;
                    obj["iconShade"] = _xml.IconShade;
                    obj["tooltip"] = expandVars(_xml.Tooltip ?? _xml.Description);
                    items.Push(obj);
                } else {
                    items.Pop((int)pair.Key);
                }

                items.SortDensePart((x, y) => {
                    AmfObject xObj = (AmfObject)x;
                    AmfObject yObj = (AmfObject)y;
                    string xName = xObj.GetString("storageName");
                    string yName = yObj.GetString("storageName");
                    return xName.CompareTo(yName);
                });

                OnPropertyChanged("Value1");
                OnPropertyChanged("Value2");
                OnPropertyChanged("Value3");
                OnPropertyChanged("Value4");
                OnPropertyChanged("Comment");
                OnSavePropertyChanged();
                OnIsOwnedChanged();
            }
        }

        protected override void NotifyGameVM() {
            _game.OnStatusChanged(Name);
        }

        protected override void OnIsOwnedChanged() {
            _game.OnStatusAddedOrRemoved(Name, IsOwned);
        }
    }
}
