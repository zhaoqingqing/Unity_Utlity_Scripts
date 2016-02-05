using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Diagnostics;
using DG.Tweening;
using SimpleJson;
using UnityEngine;


/// <summary>
/// 需要导入Dotween,NGUI
/// 
/// </summary>
public partial class UnityHelper
{
	/// <summary>
	/// 获取一个Transfrom下所有active=true的child
	/// </summary>
	public static List<GameObject> GetActiveChilds(Transform parent)
	{
		var list = new List<GameObject>();
		var max = parent.childCount;
		for (int idx = 0; idx < max; idx++)
		{
			var awardObj = parent.GetChild(idx).gameObject;
			if (awardObj.activeSelf) list.Add(awardObj);
		}
		return list;
	}

	//public static void ResizeCUITableGridGameObjects(CUITableGrid uiTable, int resizeCount, GameObject templateForNew,
	//	bool boxFixed = true)
	//{
	//	_ResizeUIWidgetContainerGameObjects(uiTable.transform, resizeCount, templateForNew);
	//	uiTable.Reposition();
	//	if (boxFixed) MakeBoxFixed(uiTable);
	//}

	//以下部分代码需要导入Dotween 和NGUI
	/*
	public static void _ResizeUIWidgetContainerGameObjects(Transform transf, int resizeCount, GameObject templateForNew)
	{
		if (templateForNew == null)
			templateForNew = default(GameObject);

		for (int i = 0; i < resizeCount; i++)
		{
			GameObject newTemplate = null;
			if (i >= transf.childCount)
			{
				newTemplate = Object.Instantiate(templateForNew) as GameObject;
				newTemplate.transform.parent = transf;
				CTool.ResetLocalTransform(newTemplate.transform);

				//gameObjList.Add(newTemplate);
			}
			newTemplate = transf.GetChild(i).gameObject;
			if (!newTemplate.activeSelf)
				newTemplate.SetActive(true);
		}

		for (int i = resizeCount; i < transf.childCount; ++i)
		{
			GameObject newTemplate = transf.GetChild(i).gameObject;
			if (newTemplate.activeSelf)
				newTemplate.SetActive(false);
		}

	}


	public static void MakeBoxFixed(CUITableGrid uiTable)
	{
		for (int i = 0; i < uiTable.transform.childCount; i++)
		{
			var trans = uiTable.transform.GetChild(i);
			var box = trans.GetComponent<BoxCollider>();
			if (box)
			{
				var boxSize = box.size;
				boxSize.y = uiTable.CellHeight;
				box.size = boxSize;
			}
		}
	}

	/// <summary>
	/// 传入指定数量， 对UIGrid里指定数量项SetActive(true)/或创建, 其余的SetActive(false)
	/// 常用于UIGrid下的对象动态增长
	/// </summary>
	public static void ResizeUIGridGameObjects(UIGrid uiGrid, int resizeCount, GameObject templateForNew = null)
	{
		if (templateForNew == null) templateForNew = uiGrid.transform.GetChild(0).gameObject;
		_ResizeUIWidgetContainerGameObjects(uiGrid.transform, resizeCount, templateForNew);
		uiGrid.Reposition();
	}

	public static void ResizeUITableGameObjects(UITable uiTable, int resizeCount, GameObject templateForNew = null)
	{
		if (templateForNew == null) templateForNew = uiTable.transform.GetChild(0).gameObject;
		_ResizeUIWidgetContainerGameObjects(uiTable.transform, resizeCount, templateForNew);
		uiTable.Reposition();
	}


	/// <summary>
	/// 弹出的面板打开动画,UI结构如下
	/// Panel 
	///      Mask
	///       Contain
	/// </summary>
	/// <param name="maskObj"></param>
	/// <param name="containTrans"></param>
	/// <param name="animFinishCallback"></param>
	public static Tween ShowPanelAnim(GameObject maskObj, Transform containTrans, Action animFinishCallback = null)
	{
		if (maskObj != null)
			maskObj.SetActive(true);
		containTrans.localScale = new Vector3(0.6f, 0.6f, 0);

		containTrans.gameObject.SetActive(true);
		return containTrans.DOScale(Vector3.one, .2f).SetEase(Ease.OutBack).OnComplete(() =>
		{
			//动画完成后的callback
			if (animFinishCallback != null)
				animFinishCallback();
		});
	}


	/// <summary>
	/// 弹出的面板关闭动画,UI结构如下
	/// Panel 
	///      Mask
	///       Contain
	/// </summary>
	/// <param name="maskObj">遮罩(允许为空)</param>
	/// <param name="containTrans">内容容器(不允许为空)</param>
	/// <param name="animFinishCallback"></param>
	public static void ClosePanelAnim(GameObject maskObj, Transform containTrans, Action animFinishCallback = null)
	{
		containTrans.localScale = Vector3.one;
		containTrans.DOScale(new Vector3(0.8f, 0.8f, 0), .2f).SetEase(Ease.InBack).OnComplete(() =>
		{
			containTrans.gameObject.SetActive(false);
			if (maskObj != null)
				maskObj.SetActive(false);
			//动画完成后的callback
			if (animFinishCallback != null)
				animFinishCallback();
		});
	}




	public static Sequence PingPongScale(GameObject target, float curScale, float endScale)
	{
		target.transform.localScale = new Vector3(curScale, curScale, 0);
		return PingPongScale(target, endScale);
	}

	public static Sequence PingPongScale(GameObject target, float scale = 1f)
	{
		target.SetActive(true);
		var curScale = target.transform.localScale;
		var endScale = new Vector3(scale, scale, 1);
		Vector3 midScale;

		// 变大, 就比更大还更大
		if (curScale.x <= endScale.x)
		{
			midScale = endScale * 1.2f;
		}
		else
		{
			midScale = endScale / 1.2f;
		}
		return DOTween.Sequence().Append(target.transform.DOScale(midScale, 0.2f))
			.Append(target.transform.DOScale(endScale, 0.1f));
	}

	public static void ScaleHide(Transform trans, Action callback)
	{
		trans.localScale = Vector3.one;
		trans.DOScale(new Vector3(0.8f, 0.8f, 0), .2f).SetEase(Ease.InBack).OnComplete(() =>
		{
			trans.gameObject.SetActive(false);
			callback();
		});
	}

	/// <summary>
	/// 动画，右边往中间靠，间隔一定时间，左边往中间靠， 然后同样方式结束
	/// </summary>
	/// <param name="right"></param>
	/// <param name="left"></param>
	/// <param name="endCallback"></param>
	public static void AnimationOfRightAndLeft(Transform right, Transform left, Action endCallback)
	{
		//TODO 屏幕中间位置使用是NGUI camera的，RepresentCamCenterPos是2dtk的camera
		//var screenCenterWorldPos = CGameCamera.Instance.RepresentCamCenterPos;
		var screenCenterWorldPos = Vector3.zero;

		var rightStartPos = screenCenterWorldPos + Vector3.right * 1;
		var leftStartPos = screenCenterWorldPos + Vector3.left * 1;
		right.position = rightStartPos;
		right.gameObject.SetActive(true);
		left.position = leftStartPos;
		left.gameObject.SetActive(true);

		var seq = DOTween.Sequence();

		seq.Insert(0,
			right.DOMove(screenCenterWorldPos, 0.2f)
				.OnComplete(() => left.DOMove(screenCenterWorldPos, 0.2f))); // 左边的，要慢一点
		seq.AppendInterval(2f); // 1

		seq.InsertCallback(2, () => left.localScale = Vector3.one * 1.4f); // 稍微放大一下
		seq.Insert(2, left.DOScale(Vector3.one, .2f));
		seq.Insert(2, right.DOMove(rightStartPos, 0.5f));
		seq.Insert(2, left.DOMove(leftStartPos, 0.5f)); // 左边的，要慢一点
		seq.OnComplete(() =>
		{
			if (endCallback != null)
				endCallback();
		});
	}



	public static void StopUITween(GameObject gameObj, int tweenGroup = -1)
	{
		// 重置出现 移动动画
		foreach (UITweener tween in gameObj.GetComponentsInChildren<UITweener>(true))
		{
			if (tweenGroup == -1 || tweenGroup == tween.tweenGroup)
			{
				tween.ResetToBeginning();
				tween.enabled = false;
			}
		}
	}

	public static void ResetUITween(GameObject gameObj, int tweenGroup = -1)
	{
		gameObj.SetActive(true);
		// 重置出现 移动动画
		foreach (UITweener tween in gameObj.GetComponentsInChildren<UITweener>(true))
		{
			if (tweenGroup == -1 || tweenGroup == tween.tweenGroup)
			{
				tween.enabled = true;
				tween.ResetToBeginning();
				tween.PlayForward();
			}
		}
	}
	 */
}