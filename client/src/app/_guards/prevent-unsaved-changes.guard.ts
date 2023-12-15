import { CanDeactivateFn } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

export const preventUnsavedChangesGuard: CanDeactivateFn<
  MemberEditComponent
> = (component, _, currentState, nextState) => {
  if (component.editForm?.dirty) {
    return confirm('Are you sure you want to continue?');
  }
  return true;
};
