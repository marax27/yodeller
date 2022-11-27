export class ComponentBuilder {
  public static create(
    tag: string,
    classes: string[],
    children: HTMLElement[]
  ): HTMLElement {
    const element = document.createElement(tag);

    if (classes != null && classes.length > 0) {
      element.classList.add(...classes);
    }

    if (children != null) {
      children.forEach((child: HTMLElement) => element.appendChild(child));
    }

    return element;
  }

  public static createIcon(iconName: string, isHuge: boolean): HTMLElement {
    const classes = isHuge ? ["huge-icon"] : [];
    const icon = this.create("ion-icon", classes, []);
    icon.setAttribute("name", iconName);
    return this.create("span", ["icon"], [icon]);
  }
}
