﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using X.Viewer.SketchFlow.Controls.Pickers;

namespace X.Viewer.SketchFlow.Controls.Stamps
{


  public sealed partial class Entity : UserControl, IStamp
  {
    public event EventHandler PerformAction;

    public Entity()
    {
      this.InitializeComponent();

      tlLeftCenterToolbar.AddTab("Colors", isSelected: true);
      tlLeftCenterToolbar.AddTab("Text");
      tlLeftCenterToolbar.OnTabChanged += TlLeftCenterToolbar_OnTabChanged;

      this.Unloaded += Entity_Unloaded;
      cpMain.ColorTypes = new List<string>() { "Stroke", "Fill" };
      tpMain.FontFamilies = new List<string>() { "Neue Haas Grotesk Text Pro", "FangSong", "Kokila", "Cambria", "Courier New", "Gadugi", "Georgia", "Leelawadee UI", "Lucida Console", "Segoe MDL2 Assets", "Segoe UI", "Segoe UI Emoji", "Verdana" };
    }

    private void TlLeftCenterToolbar_OnTabChanged(object sender, EventArgs e)
    {
      var tab = (X.UI.LiteTab.Tab)sender;

      textPicker.Visibility = Visibility.Collapsed;
      colorPicker.Visibility = Visibility.Collapsed;

      switch (tab.Name)
      {
        case "Colors": colorPicker.Visibility = Visibility.Visible; break;
        case "Text": textPicker.Visibility = Visibility.Visible; break;
      }
    }

    private void Entity_Unloaded(object sender, RoutedEventArgs e)
    {

    }

    private void edges_PerformAction(object sender, EventArgs e)
    {
      var ea = e as ResizeMoveEdgesEventArgs;

      if (ea.ActionType == eActionTypes.ToolbarTopRight)
      {
        if (spToolbar.Visibility == Visibility.Visible) spToolbar.Visibility = Visibility.Collapsed;
        else spToolbar.Visibility = Visibility.Visible;
      }
      else if (ea.ActionType == eActionTypes.CenterLeft)
      {
        if (leftCenterToolBar.Visibility == Visibility.Visible) leftCenterToolBar.Visibility = Visibility.Collapsed;
        else leftCenterToolBar.Visibility = Visibility.Visible;
      }

      PerformAction?.Invoke(this, e);
    }

    private void butClose_Click(object sender, RoutedEventArgs e)
    {
      PerformAction?.Invoke(this, new EntityEventArgs() { ActionType = eActionTypes.CloseStamp });
    }

    private void butStamp_Click(object sender, RoutedEventArgs e)
    {
      PerformAction?.Invoke(this, new EntityEventArgs() { ActionType = eActionTypes.CreateFromStamp });
    }



    public void UpdateRotation(double angle)
    {
      ((CompositeTransform)el.RenderTransform).Rotation = angle;
      //((CompositeTransform)grdGridRotationMarkers.RenderTransform).Rotation = angle;
      grdGridRotationMarkers.RotationAngle = angle;


      //    ((CompositeTransform)x1.RenderTransform).Rotation = Math.Cos(angle);
      //    ((CompositeTransform)x2.RenderTransform).Rotation = Math.Cos(angle);
      //    ((CompositeTransform)y1.RenderTransform).Rotation = Math.Cos(angle);
      //    ((CompositeTransform)y2.RenderTransform).Rotation = Math.Cos(angle);
    }

    private void cpMain_ColorChanged(object sender, EventArgs e)
    {
      var cpea = e as ColorPickerEventArgs;
      if (cpea.ColorType == "Stroke") {
        el.Stroke = (Brush)sender;
        elBgTxt.BorderBrush = (Brush)sender;
      }
      else if (cpea.ColorType == "Fill") {
        el.Fill = (Brush)sender;
        elBgTxt.Background = (Brush)sender;
      }
    }

    private void butGridMarker_Click(object sender, RoutedEventArgs e)
    {
      PerformAction?.Invoke(this, new EntityEventArgs() { ActionType = eActionTypes.ToggleGridMarkers });

      if (grdGridMarkers.Visibility == Visibility.Visible) grdGridMarkers.Visibility = Visibility.Collapsed;
      else grdGridMarkers.Visibility = Visibility.Visible;

      grdGridRotationMarkers.Visibility = grdGridMarkers.Visibility;
    }


    public string GenerateXAML(string uid, double scaleX, double scaleY, double left, double top)
    {
      var rotationAngle = ((CompositeTransform)el.RenderTransform).Rotation;
      var leftToUse = left;
      var topToUse = top;
      var rotationXaml = $"<Ellipse.RenderTransform><CompositeTransform Rotation=\"{ rotationAngle }\" /></Ellipse.RenderTransform>";
      if (rotationAngle == 0) rotationXaml = "";

      var fillColor = (el.Fill != null) ? ((SolidColorBrush)el.Fill).Color.ToString() : "";
      var fillXaml = fillColor.Length > 0 ? $"Fill=\"{fillColor}\"" : "";

      var newStroke = el.StrokeThickness * (1 / scaleX);

      var xamlBuilder = new StringBuilder();
      xamlBuilder.AppendLine($"<StackPanel x:Name=\"{uid}\" Orientation=\"Vertical\" Canvas.Left=\"{ leftToUse }\" Canvas.Top=\"{ topToUse }\" >");
      xamlBuilder.AppendLine($"<Border Background=\"{ ((SolidColorBrush)elBgTxt.Background).Color.ToString() }\" Padding=\"5,2,5,2\" Height=\"25\" HorizontalAlignment=\"Center\" VerticalAlignment=\"Top\" Margin=\"0,0,0,10\"><TextBlock Foreground=\"White\" FontSize=\"14\" Text=\"{elTxt.Text}\" TextWrapping=\"WrapWholeWords\" RenderTransformOrigin=\"0.5,0.5\" IsColorFontEnabled=\"True\"><TextBlock.RenderTransform><CompositeTransform Rotation=\"0\"></CompositeTransform></TextBlock.RenderTransform></TextBlock></Border>");
      xamlBuilder.AppendLine($"<Ellipse Height=\"{ (el.Height * (1 / scaleY)) }\" Width=\"{ (el.Width * (1 / scaleX)) }\" RenderTransformOrigin=\"0.5,0.5\" { fillXaml } >{ rotationXaml }</Ellipse>");
      xamlBuilder.AppendLine($"</StackPanel>");
      return xamlBuilder.ToString();
    }

    public void PopulateFromUIElement(UIElement element)
    {
      throw new NotImplementedException();
    }

    private void butLock_Click(object sender, RoutedEventArgs e)
    {
      butLock.Visibility = Visibility.Collapsed;
      butUnlock.Visibility = Visibility.Visible;
      edges.IsLocked = true;
    }

    private void butUnlock_Click(object sender, RoutedEventArgs e)
    {
      butLock.Visibility = Visibility.Visible;
      butUnlock.Visibility = Visibility.Collapsed;
      edges.IsLocked = false;
    }

    public void GenerateFromXAML(UIElement template)
    {
      if (template is Ellipse)
      {
        var elTemplate = template as Ellipse;
        try { ((CompositeTransform)el.RenderTransform).Rotation = ((CompositeTransform)elTemplate.RenderTransform).Rotation; } catch { }
        el.Stroke = elTemplate.Stroke;
        el.StrokeThickness = elTemplate.StrokeThickness;
        el.Fill = elTemplate.Fill;
      }
    }

    public void UpdateStrokeThickness(double thickness) { el.StrokeThickness = thickness; }
    public string GetData(string uid)
    {
      return $@"
        {{
          ""uid"":""{uid}"",
          ""displayName"":""{elTxt.Text}""
        }}
      ";      
    }

    private void tpMain_TextChanged(object sender, EventArgs e)
    {
      var tea = e as TextPickerEventArgs;
      elTxt.Text = tea.Text;
      if (!string.IsNullOrEmpty(tea.FontFamily)) elTxt.FontFamily = new FontFamily(tea.FontFamily);
    }

    public void UpdatedDimension(double width, double height)
    {

    }
    
    public void UpdatedPosition(double x, double y)
    {

    }

    public void UpdatedSelectedPage()
    {

    }

  }

  public class EntityEventArgs : EventArgs, IStampEventArgs
  {
    public eActionTypes ActionType { get; set; }

  }
}
