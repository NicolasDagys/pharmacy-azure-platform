import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  Output
} from '@angular/core';

import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-assistant-input',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './assistant-input.html',
  styleUrl: './assistant-input.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AssistantInputComponent {

  @Input()
  isDisabled = false;

  @Output()
  sendMessage = new EventEmitter<string>();

  message = '';

  submit(): void {
    const question = this.message.trim();

    if (!question || this.isDisabled) {
      return;
    }

    this.sendMessage.emit(question);
    this.message = '';
  }

  onKeyDown(event: KeyboardEvent): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.submit();
    }
  }
}