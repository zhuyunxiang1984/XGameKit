using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using Debug = UnityEngine.Debug;

[DisallowMultipleComponent]
[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class UIAutoSize : UIBehaviour, ILayoutElement
{
    
    [LabelText("原始尺寸"), OnValueChanged("_UpdateRatio"), HorizontalGroup("2"), DisableIf("_IsUseSpriteSize")]
    public Vector2 sourceSize; 
    [LabelText("使用图片尺寸"), OnValueChanged("_UpdateRatio"), LabelWidth(70), HorizontalGroup("2")]
    public bool useSpriteSize = true;
    
    [LabelText("适应尺寸"), OnValueChanged("_UpdateSize"), HorizontalGroup("3"), DisableIf("_IsUseParentSize")]
    public Vector2 screenSize;
    [LabelText("使用父框尺寸"), OnValueChanged("_UpdateSize"), LabelWidth(70), HorizontalGroup("3")]
    public bool useParentSize = true;

    bool _IsUseSpriteSize()
    {
        return useSpriteSize;
    }
    bool _IsUseParentSize()
    {
        return useParentSize;
    }
    [LabelText("图片比例"), DisplayAsString]
    public float ratio = 1f;
    [LabelText("显示完整图片")]
    public bool fullImage = false;
    [LabelText("是否使用缩放")]
    public bool useScale = false;
    
    [Button("测试")]
    void TestButton()
    {
        AutoSize();
    }
    
    private bool _IsResize = false;
    private void Update()
    {
        if (_IsResize)
        {
            AutoSize();
            _IsResize = false;
        }
    }

    #region LayoutGroup部分代码实现

    [System.NonSerialized] private RectTransform m_Rect;
    protected RectTransform rectTransform
    {
        get
        {
            if (m_Rect == null)
                m_Rect = GetComponent<RectTransform>();
            return m_Rect;
        }
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        SetDirty();
    }

    protected override void OnDisable()
    {
        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        base.OnDisable();
    }
    /// <summary>
    /// Callback for when properties have been changed by animation.
    /// </summary>
    protected override void OnDidApplyAnimationProperties()
    {
        SetDirty();
    }
    
    private bool isRootLayoutGroup
    {
        get
        {
            Transform parent = transform.parent;
            if (parent == null)
                return true;
            return transform.parent.GetComponent(typeof(ILayoutGroup)) == null;
        }
    }
    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        if (isRootLayoutGroup)
            SetDirty();
    }

    protected virtual void OnTransformChildrenChanged()
    {
        SetDirty();
    }

    /// <summary>
    /// Helper method used to set a given property if it has changed.
    /// </summary>
    /// <param name="currentValue">A reference to the member value.</param>
    /// <param name="newValue">The new value.</param>
    protected void SetProperty<T>(ref T currentValue, T newValue)
    {
        if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
            return;
        currentValue = newValue;
        SetDirty();
    }
    /// <summary>
    /// Mark the LayoutGroup as dirty.
    /// </summary>
    protected void SetDirty()
    {
        if (!IsActive())
            return;

        if (!CanvasUpdateRegistry.IsRebuildingLayout())
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        else
            StartCoroutine(DelayedSetDirty(rectTransform));
    }

    IEnumerator DelayedSetDirty(RectTransform rectTransform)
    {
        yield return null;
        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        SetDirty();
    }

#endif

    #endregion
    

    #region ILayoutElement

    public float minWidth { get; }

    public float preferredWidth { get; }

    public float flexibleWidth { get; }

    public float minHeight { get; }

    public float preferredHeight { get; }

    public float flexibleHeight { get; }

    public int layoutPriority { get; }
    
    public void CalculateLayoutInputHorizontal()
    {
        //Debug.Log("CalculateLayoutInputHorizontal");
        _IsResize = true;
    }

    public void CalculateLayoutInputVertical()
    {
        //Debug.Log("CalculateLayoutInputVertical");
        _IsResize = true;
    }

    

    #endregion
    

    public void AutoSize()
    {
        //Debug.Log("适应尺寸");
        _UpdateRatio();
        _UpdateSize();
        _AutoSize(screenSize);
    }

    Vector2 _GetSourceSize()
    {
        var image = GetComponent<Image>();
        if (image != null)
        {
            if (image.overrideSprite != null)
            {
                return new Vector2(image.overrideSprite.rect.width, image.overrideSprite.rect.height);
            }
            if (image.sprite != null)
            {
                return new Vector2(image.sprite.rect.width, image.sprite.rect.height);
            }
        }
        var rawimage = GetComponent<RawImage>();
        if (rawimage != null && rawimage.texture != null)
        {
            return new Vector2(rawimage.texture.width, rawimage.texture.height);
        }
        return rectTransform.sizeDelta;
    }
    void _UpdateRatio()
    {
        if (_IsUseSpriteSize())
        {
            sourceSize = _GetSourceSize();
        }

        if (sourceSize.x > 0f && sourceSize.y > 0f)
        {
            ratio = sourceSize.x / sourceSize.y;
        }
        else
        {
            ratio = 1;
        }
    }
    void _UpdateSize()
    {
        if (useParentSize)
        {
            var parent = transform.parent as RectTransform;
            if (parent != null)
            {
                screenSize.x = parent.rect.width;
                screenSize.y = parent.rect.height;
            }
        }
    }
    void _AutoSize(Vector2 screenSize)
    {
        if (this.ratio <= 0f)
            return;
        var w = screenSize.x;
        var h = screenSize.y;
        var rate = w / h;

        var result = Vector2.zero;

        if (fullImage)
        {
            if (rate <= ratio)
            {
                result.x = w;
                result.y = w / ratio;
            }
            else
            {
                result.y = h;
                result.x = h * ratio;
            }
        }
        else
        {
            if (rate <= ratio)
            {
                result.y = h;
                result.x = h * ratio;
            }
            else
            {
                result.x = w;
                result.y = w / ratio;
            }
        }
        if (useScale)
        {
            rectTransform.sizeDelta = sourceSize;
            rectTransform.localScale = Vector3.one * (result.x / rectTransform.sizeDelta.x);
        }
        else
        {
            rectTransform.sizeDelta = result;
            rectTransform.localScale = Vector3.one;
        }
    }
}
