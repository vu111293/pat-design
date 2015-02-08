//////////////////////////////////////////////////////////////////////////////
// Product: QF/C++
// Last Updated for Version: 4.3.00
// Date of the Last Update:  Nov 01, 2011
//
//                    Q u a n t u m     L e a P s
//                    ---------------------------
//                    innovating embedded systems
//
// Copyright (C) 2002-2011 Quantum Leaps, LLC. All rights reserved.
//
// This software may be distributed and modified under the terms of the GNU
// General Public License version 2 (GPL) as published by the Free Software
// Foundation and appearing in the file GPL.TXT included in the packaging of
// this file. Please note that GPL Section 2[b] requires that all works based
// on this software must also be made publicly available under the terms of
// the GPL ("Copyleft").
//
// Alternatively, this software may be distributed and modified under the
// terms of Quantum Leaps commercial licenses, which expressly supersede
// the GPL and are specifically designed for licensees interested in
// retaining the proprietary status of their code.
//
// Contact information:
// Quantum Leaps Web site:  http://www.quantum-leaps.com
// e-mail:                  info@quantum-leaps.com
//////////////////////////////////////////////////////////////////////////////
#include "qf_pkg.h"
#include "qassert.h"

/// \file
/// \ingroup qf
/// \brief QActive::subscribe() implementation.

#ifdef Q_USE_NAMESPACE
namespace QP {
#endif

Q_DEFINE_THIS_MODULE(qa_sub)

//............................................................................
void QActive::subscribe(QSignal sig) const {
    uint8_t p = m_prio;
    Q_REQUIRE(((QSignal)Q_USER_SIG <= sig)
              && (sig < QF_maxSignal_)
              && ((uint8_t)0 < p) && (p <= (uint8_t)QF_MAX_ACTIVE)
              && (QF::active_[p] == this));

    uint8_t i = Q_ROM_BYTE(QF_div8Lkup[p]);

    QF_CRIT_STAT_;
    QF_CRIT_ENTRY_();

    QS_BEGIN_NOCRIT_(QS_QF_ACTIVE_SUBSCRIBE, QS::aoObj_, this)
        QS_TIME_();                                               // timestamp
        QS_SIG_(sig);                              // the signal of this event
        QS_OBJ_(this);                                   // this active object
    QS_END_NOCRIT_()
                                                       // set the priority bit
    QF_subscrList_[sig].m_bits[i] |= Q_ROM_BYTE(QF_pwr2Lkup[p]);
    QF_CRIT_EXIT_();
}

#ifdef Q_USE_NAMESPACE
}                                                              // namespace QP
#endif
